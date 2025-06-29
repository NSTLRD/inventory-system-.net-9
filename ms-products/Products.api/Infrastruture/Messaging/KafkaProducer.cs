// Products.api/Infrastructure/Messaging/KafkaProducer.cs
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Products.Api.Infrastructure.Messaging
{
    public class KafkaProducer : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly ILogger<KafkaProducer> _logger;
        private static readonly ConcurrentDictionary<Guid, bool> _publishedEvents = new();

        public KafkaProducer(IConfiguration cfg, ILogger<KafkaProducer> logger)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = cfg["Kafka:BootstrapServers"],
                EnableIdempotence = true,
                Acks = Acks.All,
                MessageSendMaxRetries = 3,
                MaxInFlight = 1,
                Partitioner = Confluent.Kafka.Partitioner.Consistent
            };
            
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = cfg["Kafka:Topic"] ?? throw new ArgumentException("Kafka:Topic missing");
            _logger = logger;
        }

        public Task PublishAsync(object evt)
        {
            var evtIdProp = evt.GetType().GetProperty("Id");
            if (evtIdProp == null) return Task.CompletedTask;

            var id = (Guid)evtIdProp.GetValue(evt)!;
            
            if (!_publishedEvents.TryAdd(id, true))
            {
                _logger.LogWarning("Evento ya publicado, ignorando duplicado: {Id}", id);
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var json = JsonConvert.SerializeObject(evt);
                var message = new Message<string, string>
                {
                    Key = id.ToString(),
                    Value = json,
                    Headers = new Headers { { "EventType", Encoding.UTF8.GetBytes(evt.GetType().Name) } }
                };

                _producer.Produce(_topic, message, deliveryReport =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        _logger.LogError("Fallo al entregar mensaje: {Reason}. Key: {Key}", deliveryReport.Error.Reason, id);
                        _publishedEvents.TryRemove(id, out _); // Permitir reintento
                        tcs.SetException(new KafkaException(deliveryReport.Error));
                    }
                    else
                    {
                        _logger.LogInformation("Evento {EventType} publicado. Key: {Key}, Offset: {Offset}", evt.GetType().Name, id, deliveryReport.Offset);
                        tcs.SetResult(true);
                    }
                });
            }
            catch (Exception ex)
            {
                _publishedEvents.TryRemove(id, out _);
                _logger.LogError(ex, "Error encolando evento para publicación. Key: {Key}", id);
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
