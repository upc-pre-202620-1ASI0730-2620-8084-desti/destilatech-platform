using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class InventoryModelBuilderExtensions
{
    public static void ApplyInventoryStockConfiguration(this ModelBuilder builder)
    {
        var product = builder.Entity<Product>();
        product.HasIndex(x => x.UserId);
        product.Property(x => x.Name).HasMaxLength(120);
        product.Property(x => x.Presentation).HasMaxLength(80);
        product.Property(x => x.Unit).HasMaxLength(12);
        product.Property(x => x.Category).HasMaxLength(80);
        product.Property(x => x.Sku).HasMaxLength(64);
        product.Property(x => x.UnitPrice).HasPrecision(12, 2);

        var stock = builder.Entity<StockItem>();
        stock.HasOne<Product>().WithOne().HasForeignKey<StockItem>(x => x.ProductId);
        stock.HasIndex(x => x.UserId);
        stock.Property(x => x.CurrentQuantity).HasPrecision(12, 3);
        stock.Property(x => x.LowStockThreshold).HasPrecision(12, 3);
        stock.Property(x => x.Version).IsConcurrencyToken();

        var movement = builder.Entity<StockMovement>();
        movement.HasOne<StockItem>().WithMany().HasForeignKey(x => x.StockItemId);
        movement.HasIndex(x => new { x.UserId, x.MovementDate });
        movement.Property(x => x.MovementDate)
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        movement.Property(x => x.Quantity).HasPrecision(12, 3);
        movement.Property(x => x.Type).HasMaxLength(8);
        movement.Property(x => x.Reason).HasMaxLength(20);
        movement.Property(x => x.Notes).HasMaxLength(255);
        movement.Property(x => x.Reference).HasMaxLength(80);
    }
}