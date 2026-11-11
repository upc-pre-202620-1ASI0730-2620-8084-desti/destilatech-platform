namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Registers a replenishment order placed with a supplier.
/// </summary>
/// <param name="UserId">Account that owns the replenishment order.</param>
/// <param name="SupplierName">Supplier the products are ordered from. Required.</param>
/// <param name="SupplierContact">Phone, email or other contact of the supplier. Optional.</param>
/// <param name="ProductId">Product to replenish, from Inventory &amp; Stock Management.</param>
/// <param name="Quantity">Units ordered. Must be greater than zero.</param>
/// <param name="ExpectedDate">Date the supplier is expected to deliver. Optional.</param>
/// <param name="Notes">Free text notes. Optional.</param>
public record RequestReplenishmentOrderCommand(
    int UserId,
    string SupplierName,
    string? SupplierContact,
    int ProductId,
    int Quantity,
    DateTimeOffset? ExpectedDate,
    string? Notes);
