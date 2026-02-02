namespace ReSR.Domain.Core;

/// <summary>
/// An interface for domain events.
/// </summary>
public interface IDomainEvent {

    /// <summary>
    /// The instant at which the domain event occured.
    /// </summary>
    public DateTime OccuredAt { get; }

}