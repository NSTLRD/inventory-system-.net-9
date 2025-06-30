
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Inventory.Api.Application.Events;
using Inventory.Api.Application.Events.Common;
using Inventory.Api.Common.Interfaces;

namespace Inventory.Api.Infrastructure.Messaging
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InMemoryEventBus> _logger;
        private readonly ConcurrentDictionary<string, List<Type>> _handlers 
            = new();

        public InMemoryEventBus(IServiceScopeFactory scopeFactory,
                                ILogger<InMemoryEventBus> logger)
        {
            _scopeFactory = scopeFactory;
            _logger       = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            if (@event is IntegrationEvent integrationEvent)
            {
                await PublishIntegrationEventInternalAsync(integrationEvent, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Event {EventType} is not an IntegrationEvent and cannot be published", 
                    @event.GetType().Name);
            }
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            var handlerType = typeof(THandler);

            _handlers.AddOrUpdate(eventName,
                _ => new List<Type> { handlerType },
                (_, list) =>
                {
                    if (!list.Contains(handlerType))
                        list.Add(handlerType);
                    return list;
                });

            _logger.LogInformation("Subscribed handler {Handler} to event {Event}",
                                   handlerType.Name, eventName);
        }

        private async Task PublishIntegrationEventInternalAsync(IntegrationEvent @event, CancellationToken ct = default)
        {
            var eventName = @event.GetType().Name;
            if (!_handlers.TryGetValue(eventName, out var handlerTypes))
            {
                _logger.LogWarning("No handlers subscribed for event {Event}", eventName);
                return;
            }
            using var scope = _scopeFactory.CreateScope();
            foreach (var handlerType in handlerTypes)
            {
                try
                {
                    // Obtenemos una instancia del handler
                    var handler = scope.ServiceProvider.GetService(handlerType);
                    if (handler == null)
                    {
                        _logger.LogWarning("No handler instance found for type {HandlerType}", handlerType.Name);
                        continue;
                    }

                    // Buscamos el método Handle en el handler
                    var methodInfo = handlerType.GetMethod("Handle", 
                        new[] { @event.GetType(), typeof(CancellationToken) }) ?? 
                        handlerType.GetMethod("Handle", new[] { @event.GetType() });

                    if (methodInfo != null)
                    {
                        // Invocamos el método Handle
                        var task = methodInfo.Invoke(handler, 
                            methodInfo.GetParameters().Length > 1 
                                ? new object[] { @event, ct } 
                                : new object[] { @event }) as Task;
                        
                        if (task != null)
                        {
                            await task;
                            _logger.LogInformation("Handler {Handler} processed event {Event}", 
                                handlerType.Name, eventName);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Handler {HandlerType} does not have a compatible Handle method", 
                            handlerType.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {Event} with handler {Handler}", 
                        eventName, handlerType.Name);
                }
            }
        }
    }
}
