using System.Collections.Concurrent;
using ReSR.Domain.Core;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ReSR.Application.Services.Core.Implementations;
internal class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher {
    
    private static readonly ConcurrentDictionary<Type, Type> ListenerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task<IResponse> DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    ) {

        var response = Response.Success();
        foreach (IDomainEvent domainEvent in domainEvents) {

            using var scope = serviceProvider.CreateScope();

            Type domainEventType = domainEvent.GetType();
            Type listenerType    = DomainEventsDispatcher.ListenerTypeDictionary.GetOrAdd(
                key          : domainEventType,
                valueFactory : type => typeof(IDomainEventListener<>).MakeGenericType(type)
            );

            IEnumerable<object> listeners = scope.ServiceProvider.GetServices(listenerType).OfType<object>();

            foreach (var listener in listeners) {

                var listenerWrapper = ListenerWrapper.Create(listener, domainEventType);
                response = await response.OnSuccessAsync(() => listenerWrapper.HandleAsync(domainEvent, cancellationToken));
            }
        }

        return response;
    }

    private abstract class ListenerWrapper {

        public abstract Task<IResponse> HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
        public static ListenerWrapper Create(object listener, Type domainEventType) {

            Type wrapperType = WrapperTypeDictionary.GetOrAdd(
                key          : domainEventType,
                valueFactory : type => typeof(ListenerWrapper<>).MakeGenericType(type)
            );

            return (ListenerWrapper)Activator.CreateInstance(wrapperType, listener)!;
        }
    }

    // Generic wrapper that provides strong typing for handler invocation
    private sealed class ListenerWrapper<T>(object listener) : ListenerWrapper where T : IDomainEvent {
        
        private readonly IDomainEventListener<T> _listener = (IDomainEventListener<T>)listener;
        public override async Task<IResponse> HandleAsync(
            IDomainEvent      domainEvent,
            CancellationToken cancellationToken
        ) => await _listener.HandleAsync((T)domainEvent, cancellationToken);

    }
}
