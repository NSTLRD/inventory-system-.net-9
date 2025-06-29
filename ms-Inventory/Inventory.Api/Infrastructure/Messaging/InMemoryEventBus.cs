// Inventory.Api/Infrastructure/Messaging/InMemoryEventBus.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Inventory.Api.Application.Events;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Infrastructure.Messaging
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InMemoryEventBus> _logger;
        // Mapa: nombre del evento → lista de tipos de handler
        private readonly ConcurrentDictionary<string, List<Type>> _handlers 
            = new();

        public InMemoryEventBus(IServiceScopeFactory scopeFactory,
                                ILogger<InMemoryEventBus> logger)
        {
            _scopeFactory = scopeFactory;
            _logger       = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : IntegrationEvent
        {
            var eventName = typeof(TEvent).Name;
            if (!_handlers.TryGetValue(eventName, out var handlerTypes))
            {
                _logger.LogWarning("No handlers subscribed for event {Event}", eventName);
                return;
            }

            // Creamos un único scope y esperamos cada Handle antes de Dispose
            using var scope = _scopeFactory.CreateScope();
            foreach (var handlerType in handlerTypes)
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService(handlerType)
                    as IIntegrationEventHandler<TEvent>;

                if (handler is not null)
                {
                    // await aquí para que el DbContext siga vivo
                    await handler.Handle(@event);
                }
            }
        }
      
  

        public void Subscribe<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
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
    }
}
