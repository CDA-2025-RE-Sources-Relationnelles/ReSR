using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Application.Services.Core.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
public sealed class ManagerSessionService(
    IAccountRepository<Manager>  repository,
    IAccountAuthService<Manager> authService
) : ISessionService<Manager> {
    
    public Task<IResponse<Session<Manager>>> TryAuthAsync(string email, string password) =>
        repository
            .TryGetWithEmailAsync(email)
            .OnSuccessAsync(manager => manager
                .TryVerifyPassword(password)
                .OnSuccess(() => authService.TryGenerateToken(manager))
                .OnSuccess(token => new Session<Manager>(token, manager))
            );

    public Task<IResponse<Session<Manager>>> TryUpdateAccountAsync(Id id, string password, Func<Manager, IResponse<Manager>> transform) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(manager => manager
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryUpdateAsync(id, transform))
            ).OnSuccessAsync(manager => authService
                .TryGenerateToken(manager)
                .OnSuccess(token => new Session<Manager>(token, manager))
            );

    public Task<IResponse> TryDeleteAccountAsync(Id id, string password) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(manager => manager
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryDeleteAsync(id))
            );

}