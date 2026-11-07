using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Application;

public sealed class InventoryService(AppDbContext db, IAlertsContextFacade alerts)
{
    public IQueryable<Product> Products(int userId) =>
        db.Set<Product>().AsNoTracking().Where(x => x.UserId == userId);

    public IQueryable<StockItem> StockItems(int userId) =>
        db.Set<StockItem>().AsNoTracking().Where(x => x.UserId == userId);

    public IQueryable<StockMovement> Movements(int userId) =>
        db.Set<StockMovement>().AsNoTracking().Where(x => x.UserId == userId);

    public async Task<Product> CreateProductAsync(Product product, decimal threshold, CancellationToken ct)
    {
        if (threshold < 0 || threshold >= 1_000_000_000m || decimal.Round(threshold, 3) != threshold)
            throw new ArgumentException("Invalid low stock threshold.");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        db.Add(product);
        await db.SaveChangesAsync(ct);
        db.Add(new StockItem(product.Id, product.UserId, threshold));
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return product;
    }

    public Task<Product?> FindProductForUpdateAsync(int userId, int productId, CancellationToken ct) =>
        db.Set<Product>().SingleOrDefaultAsync(x => x.UserId == userId && x.Id == productId, ct);

    public async Task<StockItem?> ConfigureThresholdAsync(int userId, int id, decimal threshold,
        CancellationToken ct)
    {
        var stock = await db.Set<StockItem>().SingleOrDefaultAsync(x => x.UserId == userId && x.Id == id, ct);
        if (stock is null) return null;

        stock.ConfigureThreshold(threshold);
        await db.SaveChangesAsync(ct);
        await RaiseLowStockIfNeededAsync(stock, ct);
        return stock;
    }

    public async Task<StockMovement?> RegisterMovementAsync(int userId, int productId, string type,
        string reason, decimal quantity, string? notes, string? reference, CancellationToken ct)
    {
        if (!await Products(userId).AnyAsync(x => x.Id == productId && x.IsActive, ct))
            return null;

        var stock = await FindStockAsync(userId, productId, ct);
        if (stock is null) return null;

        var movement = stock.RegisterMovement(type, reason, quantity, notes, reference);
        db.Add(movement);
        await db.SaveChangesAsync(ct);
        await RaiseLowStockIfNeededAsync(stock, ct);
        return movement;
    }

    // The caller saves the shared context so bottling and the stock entry are committed together.
    public async Task<bool> StageBottledStockAsync(int userId, int productId, int units, string reference,
        CancellationToken ct)
    {
        var product = await Products(userId).SingleOrDefaultAsync(x => x.Id == productId, ct);
        if (product is not { IsActive: true, Unit: "BOTTLE" }) return false;

        var stock = await FindStockAsync(userId, productId, ct);
        if (stock is null) return false;

        db.Add(stock.RegisterMovement("IN", "PRODUCTION", units, reference: reference));
        return true;
    }

    public async Task StageOrderMovementAsync(int userId, int productId, string type, string reason,
        int quantity, string reference, CancellationToken ct)
    {
        var stock = await FindStockAsync(userId, productId, ct)
            ?? throw new InvalidOperationException("Stock item was not found for the account.");

        db.Add(stock.RegisterMovement(type, reason, quantity, reference: reference));
    }

    private Task<StockItem?> FindStockAsync(int userId, int productId, CancellationToken ct) =>
        db.Set<StockItem>().SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, ct);

    private async Task RaiseLowStockIfNeededAsync(StockItem stock, CancellationToken ct)
    {
        if (stock.IsBelowThreshold())
            await alerts.RaiseLowStockAlert(stock.UserId, stock.ProductId,
                $"Product {stock.ProductId} is at or below its stock threshold.", ct);
    }
}