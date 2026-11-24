using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email ?? string.Empty, resource.Password ?? string.Empty);
    }
}
