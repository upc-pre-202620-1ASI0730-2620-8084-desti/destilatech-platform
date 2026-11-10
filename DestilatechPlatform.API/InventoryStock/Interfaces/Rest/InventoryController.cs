using System.Security.Claims;
using DestilatechPlatform.API.InventoryStock.Application;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

[ApiController]
[Route("api/v1")]
[Tags("Inventory")]
public sealed class InventoryController(InventoryService inventory, AppDbContext db) : ControllerBase
{
    public sealed record ProductRequest(string Name, string? Presentation, string Unit,
        string? Category, decimal UnitPrice, string? Sku, decimal LowStockThreshold = 0);

    public sealed record MovementRequest(int ProductId, string Type, string Reason, decimal Quantity,
        string? Notes, string? Reference);

    public sealed record ThresholdRequest(decimal LowStockThreshold);

    private int ResolveUserId(int requested)
    {
        if (User.Identity?.IsAuthenticated == true)
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
                out var id) && id > 0 ? id : 0;

        return requested > 0 ? requested : 0;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] int userId, CancellationToken ct) =>
        ResolveUserId(userId) is var owner && owner > 0
            ? Ok(await inventory.Products(owner).OrderBy(x => x.Id).ToListAsync(ct)) : Unauthorized();

    [HttpGet("stock-items")]
    public async Task<IActionResult> GetStock([FromQuery] int userId, CancellationToken ct) =>
        ResolveUserId(userId) is var owner && owner > 0
            ? Ok(await inventory.StockItems(owner).OrderBy(x => x.Id).ToListAsync(ct)) : Unauthorized();

    [HttpGet("stock-movements")]
    public async Task<IActionResult> GetMovements([FromQuery] int userId, CancellationToken ct) =>
        ResolveUserId(userId) is var owner && owner > 0
            ? Ok(await inventory.Movements(owner).OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id).ToListAsync(ct)) : Unauthorized();

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromQuery] int userId, ProductRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var product = new Product(owner, request.Name, request.Presentation, request.Unit,
                request.Category, request.UnitPrice, request.Sku);
            await inventory.CreateProductAsync(product, request.LowStockThreshold, ct);
            return StatusCode(201, product);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("products/{id:int}")]
    public async Task<IActionResult> UpdateProduct([FromQuery] int userId, int id, ProductRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        var product = await inventory.FindProductForUpdateAsync(owner, id, ct);
        if (product is null) return NotFound();

        try
        {
            product.Edit(request.Name, request.Presentation, request.Unit, request.Category, request.UnitPrice,
                request.Sku);
            await db.SaveChangesAsync(ct);
            return Ok(product);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("stock-items/{id:int}")]
    public async Task<IActionResult> UpdateThreshold([FromQuery] int userId, int id, ThresholdRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        try
        {
            var stock = await inventory.ConfigureThresholdAsync(owner, id, request.LowStockThreshold, ct);
            return stock is null ? NotFound() : Ok(stock);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (DbUpdateConcurrencyException) { return Conflict("Stock changed. Refresh and try again."); }
    }

    [HttpPost("stock-movements")]
    public async Task<IActionResult> RegisterMovement([FromQuery] int userId, MovementRequest request,
        CancellationToken ct)
    {
        var owner = ResolveUserId(userId);
        if (owner == 0) return Unauthorized();

        if (request.Reason is "PRODUCTION" or "ORDER" or "RETURN")
            return BadRequest("Use the source workflow for this movement.");

        try
        {
            var movement = await inventory.RegisterMovementAsync(owner, request.ProductId, request.Type,
                request.Reason, request.Quantity, request.Notes, request.Reference, ct);
            return movement is null ? NotFound() : StatusCode(201, movement);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (DbUpdateConcurrencyException) { return Conflict("Stock changed. Refresh and try again."); }
        catch (InvalidOperationException ex) when (ex.Message is "Insufficient stock." or "Maximum stock exceeded.")
        {
            return Conflict(ex.Message);
        }
    }
}