namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Registers a customer order and discounts its products from stock (US18).
/// </summary>
/// <param name="UserId">Account that owns the order.</param>
/// <param name="CustomerId">Customer that places the order.</param>
/// <param name="Lines">Products and quantities of the order. At least one is required.</param>
/// <param name="Notes">Free text notes. Optional.</param>
public record RegisterOrderCommand(int UserId, int CustomerId, IReadOnlyCollection<OrderLineData> Lines, string? Notes);
