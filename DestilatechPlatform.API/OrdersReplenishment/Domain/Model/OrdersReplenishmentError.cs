namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model;

/// <summary>
/// Business error of the Orders &amp; Replenishment bounded context.
/// </summary>
/// <remarks>
/// The codes match the <c>orders.errors.*</c> translation keys of the web application,
/// so the REST layer can return them and the front end can show the localized message.
/// </remarks>
/// <param name="Code">Stable error code, in kebab case.</param>
/// <param name="Message">English description of the error.</param>
public sealed record OrdersReplenishmentError(string Code, string Message)
{
    public static OrdersReplenishmentError InvalidData(string message) =>
        new("invalid-data", message);

    public static OrdersReplenishmentError NameRequired() =>
        new("name-required", "Enter the customer name.");

    public static OrdersReplenishmentError ContactRequired() =>
        new("contact-required", "Enter at least a phone number or an email.");

    public static OrdersReplenishmentError CustomerRequired() =>
        new("customer-required", "Select a customer.");

    public static OrdersReplenishmentError LinesRequired() =>
        new("lines-required", "Add at least one product with a quantity.");

    public static OrdersReplenishmentError ProductRequired() =>
        new("product-required", "Select a valid product.");

    public static OrdersReplenishmentError QuantityRequired() =>
        new("quantity-required", "The quantity must be greater than zero.");

    public static OrdersReplenishmentError InsufficientStock() =>
        new("insufficient-stock", "The order exceeds the available stock.");

    public static OrdersReplenishmentError OrderClosed() =>
        new("order-closed", "The order is already closed.");

    public static OrdersReplenishmentError InvalidStatusTransition(string from, string to) =>
        new("invalid-status-transition", $"An order cannot change from {from} to {to}.");

    public static OrdersReplenishmentError SupplierRequired() =>
        new("supplier-required", "Enter the supplier.");

    public static OrdersReplenishmentError ReplenishmentClosed() =>
        new("replenishment-closed", "The replenishment is already closed.");

    public static OrdersReplenishmentError CustomerNotFound(int customerId) =>
        new("customer-not-found", $"Customer {customerId} was not found.");

    public static OrdersReplenishmentError OrderNotFound(int orderId) =>
        new("order-not-found", $"Order {orderId} was not found.");

    public static OrdersReplenishmentError ReplenishmentOrderNotFound(int replenishmentOrderId) =>
        new("replenishment-not-found", $"Replenishment order {replenishmentOrderId} was not found.");
}
