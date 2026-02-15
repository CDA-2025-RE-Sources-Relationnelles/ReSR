using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IManagerCommandService {

    public Task<IResponse<Manager>> TryCreateAsync(
        string email,
        string password,
        ManagerPermissions managerPermissions
    );
    
    public Task<IResponse<Manager>> TryUpdateAsync(
        Id id,
        string? email = null,
        string? password = null,
        ManagerPermissions? managerPermissions = null
    );

    public Task<IResponse> TryDeleteAsync(Id id);

}
