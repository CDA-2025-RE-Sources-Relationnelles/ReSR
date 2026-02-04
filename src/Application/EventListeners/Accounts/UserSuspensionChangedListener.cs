using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners.Accounts;
internal class UserSuspensionChangedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserSuspensionChanged> {
    
    public Task<IResponse> HandleAsync(UserSuspensionChanged domainEvent, CancellationToken cancellationToken = default) =>
        repository.TryGetAsync(domainEvent.UserId).OnSuccessAsync(user => domainEvent.Suspended
            ? mailService.TrySendEmailAsync(
                user.Email,
                "Suspension de votre compte RE(Sources) Relationnelles",
                $"""
                Votre compte RE(Sources) Relationnelles a été temporairement suspendu.
                """
            ) : mailService.TrySendEmailAsync(
                user.Email,
                "Suspension de votre compte RE(Sources) Relationnelles",
                $"""
                Votre compte RE(Sources) Relationnelles est de nouveau actif.
                """
            ));
}