using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Logging;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class UserCommandService(
    IRepository<User> repository,
    ILogger<UserCommandService> logger
) : IUserCommandService {

    public Task<IResponse<User>> TryCreateAsync(
        string username,
        string email,
        string password,
        UserPermissions permissions
    ) => User
        .TryCreate(username, email, password, permissions)
        .OnSuccessAsync(repository.TryAddAsync)
        .OnSuccessAsync(x => logger.LogInformation("User created by manager: {@User} !", x));
    
    public Task<IResponse<User>> TryUpdateAsync(
        Id id,
        string? username = null,
        string? email = null,
        UserPermissions? permissions = null
    ) => repository.TryUpdateAsync(id, manager => {

        var response = Response.Success(manager);

        if (username is not null) response = response.OnSuccess(x => x.TryWithUsername(username));
        if (email is not null) response = response.OnSuccess(x => x.TryWithMailAddress(email));
        if (permissions is not null) response = response.OnSuccess(x => x.WithPermissions(permissions.Value));

        return response;

    }).OnSuccessAsync(x => logger.LogInformation("User updated by manager: {@User} !", x));

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository
            .TryDeleteAsync(id)
            .OnSuccessAsync(() => logger.LogInformation("User with id {@Id} deleted by manager !", id));

    public Task<IResponse<User>> TryAnonymizeAsync(Id id) =>
        repository
            .TryUpdateAsync(id, x => x.AsAnonymized())
            .OnSuccessAsync(x => logger.LogInformation("User anonymized by manager: {@User} !", x));

}
