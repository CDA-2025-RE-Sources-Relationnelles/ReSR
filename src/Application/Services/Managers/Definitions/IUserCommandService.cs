using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IUserCommandService {

    public Task<IResponse<User>> TryCreateAsync(
        string username,
        string email,
        string password,
        UserPermissions permissions
    );
    
    public Task<IResponse<User>> TryUpdateAsync(
        Id id,
        string? username = null,
        string? email = null,
        UserPermissions? permissions = null
    );

    public Task<IResponse> TryDeleteAsync(Id id);
    public Task<IResponse<User>> TryAnonymizeAsync(Id id);

}
