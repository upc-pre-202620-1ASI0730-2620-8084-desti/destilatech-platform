using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        return new UserResource(
            user.Id,
            user.Name,
            user.Email,
            user.BusinessName,
            user.UserType.ToString().ToLowerInvariant(),
            user.PreferredPlan,
            user.TrialStartedAt,
            user.TrialEndsAt,
            user.AccessGrantedUntil,
            user.CreatedAt);
    }
}