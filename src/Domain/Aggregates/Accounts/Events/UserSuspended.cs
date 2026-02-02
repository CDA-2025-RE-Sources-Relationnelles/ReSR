using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record UserSuspensionChanged(
    Id   UserId,
    bool Value
) : DomainEvent;