using DestilatechPlatform.API.Shared.Domain.Model.Errors;

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
/// <param name="Type">Kind of error, used by the REST layer to choose the HTTP status code.</param>
public sealed record OrdersReplenishmentError(string Code, string Message, EErrorType Type = EErrorType.Validation)
    : Error(Code, Message, Type)
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
        new("insufficient-stock", "The order exceeds the available stock.", EErrorType.Conflict);

    public static OrdersReplenishmentError OrderClosed() =>
        new("order-closed", "The order is already closed.", EErrorType.Conflict);

    public static OrdersReplenishmentError InvalidStatusTransition(string from, string to) =>
        new("invalid-status-transition", $"An order cannot change from {from} to {to}.", EErrorType.Conflict);

    public static OrdersReplenishmentError SupplierRequired() =>
        new("supplier-required", "Enter the supplier.");

    public static OrdersReplenishmentError ReplenishmentClosed() =>
        new("replenishment-closed", "The replenishment is already closed.", EErrorType.Conflict);

    public static OrdersReplenishmentError CustomerNotFound(int customerId) =>
        new("customer-not-found", $"Customer {customerId} was not found.", EErrorType.NotFound);

    public static OrdersReplenishmentError OrderNotFound(int orderId) =>
        new("order-not-found", $"Order {orderId} was not found.", EErrorType.NotFound);

    public static OrdersReplenishmentError ReplenishmentOrderNotFound(int replenishmentOrderId) =>
        new("replenishment-not-found", $"Replenishment order {replenishmentOrderId} was not found.", EErrorType.NotFound);
}
