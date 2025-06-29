// Inventory.Api/Infrastructure/Messaging/KafkaEventBus.cs
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Inventory.Api.Application.Events;
using Inventory.Api.Infrastructure.Configuration;
using System.Collections.Concurrent;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Infrastructure.Messaging
{
    public class KafkaEventBus : IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaSettings _settings;
        private readonly ConcurrentDictionary<Guid, bool> _processedEvents = new();

        public KafkaEventBus(IOptions<KafkaSettings> opts)
        {
            _settings = opts.Value;
            var prodConfig = new ProducerConfig {
                BootstrapServers = _settings.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(prodConfig).Build();
        }

        public Task PublishAsync<TEvent>(
            TEvent @event,
            CancellationToken cancellationToken
        ) where TEvent : IntegrationEvent
        {
            if (_processedEvents.ContainsKey(@event.Id))
            {
                // Ya se procesó este evento, no lo publicamos nuevamente
                return Task.CompletedTask;
            }

            _processedEvents[@event.Id] = true;

            var json = JsonSerializer.Serialize(@event);
            var tcs  = new TaskCompletionSource<bool>();

            _producer.Produce(
                _settings.Topic,
                new Message<Null, string> { Value = json },
                r =>
                {
                    if (r.Error.IsError) tcs.SetException(new Exception(r.Error.Reason));
                    else                 tcs.SetResult(true);
                }
            );

            return tcs.Task;
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
