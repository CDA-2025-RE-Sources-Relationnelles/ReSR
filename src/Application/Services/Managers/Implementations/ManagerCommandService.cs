using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Logging;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class ManagerCommandService(
    IRepository<Manager> repository,
    ILogger<ManagerCommandService> logger
) : IManagerCommandService {

    public Task<IResponse<Manager>> TryCreateAsync(
        string email,
        string password,
        ManagerPermissions managerPermissions
    ) => Manager
        .TryCreate(email, password, managerPermissions)
        .OnSuccessAsync(repository.TryAddAsync)
        .OnSuccessAsync(x => logger.LogInformation("Manager created by manager: {@Manager} !", x));
    
    public Task<IResponse<Manager>> TryUpdateAsync(
        Id id,
        string? email = null,
        string? password = null,
        ManagerPermissions? permissions = null
    ) => repository.TryUpdateAsync(id, manager => {

        var response = Response.Success(manager);

        if (email is not null) response = response.OnSuccess(x => x.TryWithMailAddress(email));
        if (password is not null) response = response.OnSuccess(x => x.TryWithPassword(password));
        if (permissions is not null) response = response.OnSuccess(x => x.WithPermissions(permissions.Value));

        return response;

    }).OnSuccessAsync(x => logger.LogInformation("Manager updated by manager: {@Manager} !", x));

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository
            .TryDeleteAsync(id)
            .OnSuccessAsync(() => logger.LogInformation("Manager with id {@Id} deleted by manager !", id));

}
