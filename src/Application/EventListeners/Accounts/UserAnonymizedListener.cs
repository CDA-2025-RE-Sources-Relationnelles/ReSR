using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners.Accounts;
internal class UserAnonymizedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserAnonymized> {
    
    public Task<IResponse> HandleAsync(UserAnonymized domainEvent, CancellationToken cancellationToken = default) =>
        repository.TryGetAsync(domainEvent.UserId).OnSuccessAsync(user => mailService.TrySendEmailAsync(
            user.Email,
            "Désactivation de votre compte RE(Sources) Relationnelles",
            $"""
            Votre compte RE(Sources) Relationnelles a été désactivé et anonymisé.
            """
        ));
}