namespace ReSR.Domain.Core;

/// <summary>
/// An interface for aggregate roots.
/// </summary>
/// <typeparam name="T">The aggregate root's type.</typeparam>
public interface IAggregateRoot<T> where T : IAggregateRoot<T> {

    /// <summary>
    /// The aggregate root's unique identifier.
    /// </summary>
    public Id Id { get;}

    /// <summary>
    /// The aggregate root's unconsumed events.
    /// </summary>
    public IEnumerable<IDomainEvent> DomainEvents { get; }


    /// <summary>
    /// Consumes the aggregate root's domain events.
    /// </summary>
    /// <returns>A copy of the aggregate root with its domain events consumed.</returns>
    /// <param name="domainEvents">The consumed domain events.</param>
    public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents);

}