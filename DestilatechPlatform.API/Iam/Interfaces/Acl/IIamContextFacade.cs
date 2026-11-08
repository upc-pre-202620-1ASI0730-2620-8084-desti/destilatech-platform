namespace DestilatechPlatform.API.Iam.Interfaces.Acl;

public interface IIamContextFacade
{
    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> ExtendUserAccessAsync(int userId, DateTimeOffset until,
        CancellationToken cancellationToken = default);
}