using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;

/// <summary>
/// Turns the IAM errors into Problem Details (RFC 9457) responses.
/// </summary>
public static class IamProblemAssembler
{
    public static ObjectResult ToIamProblem(this ControllerBase controller, IamError error)
    {
        return error switch
        {
            IamError.InvalidData => controller.Problem(
                "Datos inválidos: revisa nombre, correo, negocio y tipo de usuario.",
                statusCode: StatusCodes.Status400BadRequest, title: "Invalid data"),
            IamError.WeakPassword => controller.Problem(
                "La contraseña debe tener al menos 8 caracteres.",
                statusCode: StatusCodes.Status400BadRequest, title: "Weak password"),
            IamError.EmailAlreadyExists => controller.Problem("Ya existe un usuario con ese correo.",
                statusCode: StatusCodes.Status409Conflict, title: "Email already exists"),
            IamError.InvalidCredentials => controller.Problem("El correo o la contraseña no son correctos.",
                statusCode: StatusCodes.Status401Unauthorized, title: "Invalid credentials"),
            IamError.UserNotFound => controller.Problem("Usuario no encontrado.",
                statusCode: StatusCodes.Status404NotFound, title: "User not found"),
            _ => controller.Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
