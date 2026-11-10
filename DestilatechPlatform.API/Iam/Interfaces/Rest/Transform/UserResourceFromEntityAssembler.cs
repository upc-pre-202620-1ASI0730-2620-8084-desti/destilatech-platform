using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        return new UserResource(
            user.Id,
            user.FullName,
            user.Email,
            user.BusinessName,
            user.BusinessType.ToString().ToUpperInvariant(),
            user.CreatedAt,
            new TrialPeriodResource(user.TrialStartedAt, user.TrialEndsAt),
            user.AccessGrantedUntil,
            user.PreferredPlanCode);
    }
}