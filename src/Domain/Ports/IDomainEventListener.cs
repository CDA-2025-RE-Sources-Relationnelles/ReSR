using FluentResponse.Interfaces;
using ReSR.Domain.Core;

namespace ReSR.Domain.Ports;

/// <summary>
/// A service for handling domain event handling.
/// </summary>
/// <typeparam name="T">The domain event's type.</typeparam>
public interface IDomainEventListener<T> where T : IDomainEvent {

    /// <summary>
    /// Handles the given domain event.
    /// </summary>
    /// <param name="domainEvent">The domain events to handle.</param>
    public Task<IResponse> HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
    
}