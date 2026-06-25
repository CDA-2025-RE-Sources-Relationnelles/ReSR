using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record UserSuspensionChanged(
    Id      UserId,
    bool    Suspended,
    string? Reason
) : DomainEvent;