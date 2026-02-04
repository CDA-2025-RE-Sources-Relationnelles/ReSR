using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners.Accounts;
internal class UserAnonymizationProcessStartedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserAnonymizationProcessStarted> {
    
    public Task<IResponse> HandleAsync(UserAnonymizationProcessStarted domainEvent, CancellationToken cancellationToken = default) =>
        repository.TryGetAsync(domainEvent.UserId).OnSuccessAsync(user => mailService.TrySendEmailAsync(
            user.Email,
            "Désactivation de votre compte RE(Sources) Relationnelles",
            $"""
            Votre compte RE(Sources) Relationnelles sera désactivé et anonymisé le {domainEvent.OccuredAt.AddMonths(1).Date} pour cause d'inactivité.
            Connectez-vous à l'application pour annuler le processus.
            """
        ));
}