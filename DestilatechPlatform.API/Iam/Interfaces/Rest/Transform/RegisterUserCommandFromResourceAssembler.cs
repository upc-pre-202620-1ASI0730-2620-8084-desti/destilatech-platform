using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;

public static class RegisterUserCommandFromResourceAssembler
{
    public static RegisterUserCommand ToCommandFromResource(RegisterUserResource resource)
    {
        return new RegisterUserCommand(
            resource.FullName ?? string.Empty,
            resource.Email ?? string.Empty,
            resource.BusinessName ?? string.Empty,
            resource.BusinessType ?? string.Empty,
            resource.PreferredPlanCode);
    }
}