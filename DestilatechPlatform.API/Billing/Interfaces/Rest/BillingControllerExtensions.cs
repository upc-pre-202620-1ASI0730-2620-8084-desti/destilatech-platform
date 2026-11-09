using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

public static class BillingControllerExtensions
{
    public static ObjectResult ToProblem(this ControllerBase controller, BillingError error)
    {
        return error switch
        {
            BillingError.InvalidData => controller.Problem("Datos inválidos.",
                statusCode: StatusCodes.Status400BadRequest, title: "Invalid data"),
            BillingError.UserNotFound => controller.Problem("Usuario no encontrado.",
                statusCode: StatusCodes.Status404NotFound, title: "User not found"),
            BillingError.PlanNotFound => controller.Problem("Plan no encontrado.",
                statusCode: StatusCodes.Status404NotFound, title: "Plan not found"),
            BillingError.SubscriptionNotFound => controller.Problem("Suscripción no encontrada.",
                statusCode: StatusCodes.Status404NotFound, title: "Subscription not found"),
            BillingError.SessionNotFound => controller.Problem("Sesión de pago no encontrada.",
                statusCode: StatusCodes.Status404NotFound, title: "Checkout session not found"),
            BillingError.PaymentNotCompleted => controller.Problem("El pago aún no se completó.",
                statusCode: StatusCodes.Status402PaymentRequired, title: "Payment not completed"),
            BillingError.AmountMismatch => controller.Problem("El monto cobrado no coincide con el plan.",
                statusCode: StatusCodes.Status409Conflict, title: "Amount mismatch"),
            BillingError.PaymentGatewayFailure => controller.Problem("Error al comunicarse con Stripe.",
                statusCode: StatusCodes.Status502BadGateway, title: "Payment gateway failure"),
            _ => controller.Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}