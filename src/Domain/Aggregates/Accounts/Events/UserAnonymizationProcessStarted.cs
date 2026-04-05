using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record UserAnonymizationProcessStarted(
    Id UserId
) : DomainEvent;