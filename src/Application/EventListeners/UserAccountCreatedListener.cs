using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners;
public sealed class UserAccountCreatedListener(
    IMailService mailService
) : IDomainEventListener<AccountCreated<User>> {
    
    public Task<IResponse> HandleAsync(AccountCreated<User> domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.AtEmail,
            "Votre compte RE(Sources) Relationnelles a été créé",
            """
            Bonjour,
            Votre compte RE(Sources) Relationnelles a été créé avec succès.
            """
        );
}