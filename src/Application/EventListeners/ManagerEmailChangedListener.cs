using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners;
internal class ManagerEmailChangedListener(
    IMailService mailService
) : IDomainEventListener<AccountEmailChanged<Manager>> {
    
    public Task<IResponse> HandleAsync(AccountEmailChanged<Manager> domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.NewEmail,
            "Mise à jour de votre compte manager RE(Sources) Relationnelles",
            $"""
            Bonjour,
            L'addresse électronique de votre compte manager RE(Sources) Relationnelles a été changée avec succsès de {domainEvent.OldEmail} vers {domainEvent.NewEmail}.
            """
        );
}