using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners.Resources;
internal class ResourceVerifiedListener(
    IRepository<Resource> repository,
    IMailService          mailService
) : IDomainEventListener<ResourceVerified> {
    
    public Task<IResponse> HandleAsync(ResourceVerified domainEvent, CancellationToken cancellationToken = default) =>
        repository
            .TryGetAsync(domainEvent.ResourceId)
            .OnSuccessAsync(async resource => resource.Owner is User owner
                ? await mailService.TrySendEmailAsync(
                    owner.Email,
                    "Publication de votre ressource RE(Sources) Relationnelles",
                    $"""
                    La demande de publication de votre ressource {resource.Title} a été acceptée.
                    """
                ) : Response.Success()
            );

}