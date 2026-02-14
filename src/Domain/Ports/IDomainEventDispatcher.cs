using FluentResponse.Interfaces;
using ReSR.Domain.Core;

namespace ReSR.Domain.Ports;

/// <summary>
/// A service for handling domain event dispatching.
/// </summary>
public interface IDomainEventDispatcher {

    /// <summary>
    /// Dispatches the given domain events, and waits for all of the listeners to respond.
    /// </summary>
    /// <returns>A successful response if all the listeners' callback were successful.</returns>
    /// <param name="domainEvents">The domain events to dispatch.</param>
    public Task<IResponse> DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

}