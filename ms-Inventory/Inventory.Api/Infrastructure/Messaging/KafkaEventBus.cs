
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Inventory.Api.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Inventory.Api.Infrastructure.Configuration;

namespace Inventory.Api.Infrastructure.Messaging
{
    public class KafkaEventBus : Inventory.Api.Common.Interfaces.IEventBus
    {
        private readonly IProducer<string, string> _producer;
        private readonly KafkaSettings _kafkaConfig;
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<string, List<Type>> _eventHandlers = new();

        public KafkaEventBus(
            IOptions<KafkaSettings> kafkaOptions, 
            IServiceProvider serviceProvider)
        {
            _kafkaConfig = kafkaOptions.Value;
            _serviceProvider = serviceProvider;

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : class
        {
            var eventName = @event.GetType().Name;
            var topic = $"{_kafkaConfig.TopicPrefix ?? "product"}.{eventName}";
            var key = Guid.NewGuid().ToString();
            var value = JsonSerializer.Serialize(@event);
            
            var message = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            await _producer.ProduceAsync(topic, message, cancellationToken);
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Type>();
            }
            
            if (_eventHandlers[eventName].Contains(typeof(THandler)))
            {
                throw new ArgumentException(
                    $"El handler {typeof(THandler).Name} ya está registrado para el evento {eventName}");
            }
            
            _eventHandlers[eventName].Add(typeof(THandler));
        }
    }

}
