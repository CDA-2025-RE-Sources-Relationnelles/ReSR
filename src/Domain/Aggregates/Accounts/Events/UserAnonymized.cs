using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record UserAnonymized(
    Id UserId
) : DomainEvent;