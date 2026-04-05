using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record AccountCreated<T>(
    string AtEmail
) : DomainEvent where T : Account<T>;