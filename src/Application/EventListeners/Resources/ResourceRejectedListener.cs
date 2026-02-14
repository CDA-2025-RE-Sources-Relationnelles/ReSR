using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.Events;
using ReSR.Domain.Ports;

namespace ReSR.Application.EventListeners.Resources;
internal class ResourceRejectedListener(
    IRepository<Resource> repository,
    IMailService          mailService
) : IDomainEventListener<ResourceRejected> {
    
    public Task<IResponse> HandleAsync(ResourceRejected domainEvent, CancellationToken cancellationToken = default) =>
        repository
            .TryGetAsync(domainEvent.ResourceId)
            .OnSuccessAsync(async resource => resource.Owner is User owner
                ? await mailService.TrySendEmailAsync(
                    owner.Email,
                    "Refus de publication de votre ressource RE(Sources) Relationnelles",
                    $"""
                    La demande de publication de votre ressource {resource.Title} a été réfusée.
                    """
                ) : Response.Success()
            );

}