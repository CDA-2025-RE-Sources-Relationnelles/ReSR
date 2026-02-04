using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners;
internal class ManagerAccountCreatedListener(
    IMailService mailService
) : IDomainEventListener<AccountCreated<Manager>> {

    public Task<IResponse> HandleAsync(AccountCreated<Manager> domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.AtEmail,
            "Votre compte manager RE(Sources) Relationnelles a été créé",
            """
            Bonjour,
            Votre compte manager RE(Sources) Relationnelles a été créé avec succès.
            """
        );
}