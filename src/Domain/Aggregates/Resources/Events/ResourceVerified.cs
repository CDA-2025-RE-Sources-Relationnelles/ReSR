using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Resources.Events;
public record ResourceVerified(
    Id ResourceId
) : DomainEvent;