using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record AccountEmailChanged<T>(
    Id     AccountId,
    string OldEmail,
    string NewEmail
) : DomainEvent where T : Account<T>;