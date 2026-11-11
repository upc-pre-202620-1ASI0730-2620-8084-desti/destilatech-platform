namespace DestilatechPlatform.API.Iam.Interfaces.Acl;

public interface IIamContextFacade
{
    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> ExtendUserAccessAsync(int userId, DateTimeOffset until,
        CancellationToken cancellationToken = default);

    /// <returns>PRODUCER or RETAILER, or <c>null</c> if the user does not exist.</returns>
    Task<string?> FetchBusinessTypeAsync(int userId, CancellationToken cancellationToken = default);
}