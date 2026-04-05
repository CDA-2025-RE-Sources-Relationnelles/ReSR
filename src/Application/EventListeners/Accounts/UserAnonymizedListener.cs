using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Ports;

namespace ReSR.Application.EventListeners.Accounts;
internal class UserAnonymizedListener(
    IMailService mailService
) : IDomainEventListener<UserAnonymized> {
    
    public Task<IResponse> HandleAsync(UserAnonymized domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.UserEmail,
            "Désactivation de votre compte RE(Sources) Relationnelles",
            $"""
            Votre compte RE(Sources) Relationnelles a été désactivé et anonymisé.
            """
        );
}