namespace ReSR.Domain.Core;

/// <summary>
/// A domain event base record.
/// </summary>
public abstract record DomainEvent : IDomainEvent {
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}