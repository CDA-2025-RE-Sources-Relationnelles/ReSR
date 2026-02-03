using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Resources.Events;
public record ResourceRejected(
    Id ResourceId
) : DomainEvent;