using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners;
internal class UserEmailChangedListener(
    IMailService mailService
) : IDomainEventListener<AccountEmailChanged<User>> {
    
    public Task<IResponse> HandleAsync(AccountEmailChanged<User> domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.NewEmail,
            "Mise à jour de votre compte RE(Sources) Relationnelles",
            $"""
            Bonjour,
            L'addresse électronique de votre compte RE(Sources) Relationnelles a été changée avec succsès de {domainEvent.OldEmail} vers {domainEvent.NewEmail}.
            """
        );
}