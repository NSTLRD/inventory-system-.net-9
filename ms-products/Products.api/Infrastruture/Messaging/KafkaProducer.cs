
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka.Admin;

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
            var bootstrapServers = cfg["Kafka:BootstrapServers"] ?? "kafka:9092";
            _topic = cfg["Kafka:Topic"] ?? "product-events";
            
            if (string.IsNullOrEmpty(_topic))
                throw new ArgumentException("Kafka:Topic no puede estar vacío");
                
            _logger = logger;
            
            _logger.LogInformation("Intentando crear/verificar el topic {Topic} en {BootstrapServers}", 
                _topic, bootstrapServers);
                
            CreateTopicIfNotExists(_topic, bootstrapServers);
            
           
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                EnableIdempotence = true,
                Acks = Acks.All,
                MessageSendMaxRetries = 5,  
                RetryBackoffMs = 100,       
                MaxInFlight = 1,
                Partitioner = Confluent.Kafka.Partitioner.Consistent,
                SocketTimeoutMs = 10000,   
                MessageTimeoutMs = 30000    
            };
            
            _producer = new ProducerBuilder<string, string>(config).Build();
            
            _logger.LogInformation("Productor Kafka inicializado para topic: {Topic}", _topic);
        }

        private void CreateTopicIfNotExists(string topicName, string bootstrapServers)
        {
            try
            {
                using var adminClient = new AdminClientBuilder(
                    new AdminClientConfig { 
                        BootstrapServers = bootstrapServers,
                        SocketTimeoutMs = 5000 
                    }).Build();
                
                
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var topicExists = metadata.Topics.Any(t => t.Topic == topicName);
                
                if (topicExists)
                {
                    _logger.LogInformation("Topic {TopicName} ya existe", topicName);
                    return;
                }
                
                var topicSpec = new TopicSpecification
                {
                    Name = topicName,
                    ReplicationFactor = 1,
                    NumPartitions = 3,
                    Configs = new Dictionary<string, string>
                    {
                        { "cleanup.policy", "delete" },
                        { "min.insync.replicas", "1" },
                        { "retention.ms", "604800000" }  // 1 semana
                    }
                };
                
                var options = new CreateTopicsOptions 
                { 
                    OperationTimeout = TimeSpan.FromSeconds(60)
                };
                
                // Intentar crear el tema
                adminClient.CreateTopicsAsync(new[] { topicSpec }, options).GetAwaiter().GetResult();
                
                _logger.LogInformation("Topic {TopicName} creado exitosamente", topicName);
            }
            catch (CreateTopicsException ex) when (ex.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                _logger.LogInformation("Topic {TopicName} ya existe según la respuesta de Kafka", topicName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo crear el topic {TopicName} automáticamente: {Error}. Continuando sin crear el topic.", 
                    topicName, ex.Message);
                // La creación del topic fallida se manejará en tiempo de ejecución si es necesario
            }
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
                var eventType = evt.GetType().Name;
                
                var message = new Message<string, string>
                {
                    Key = id.ToString(),
                    Value = json,
                    Headers = new Headers { 
                        { "EventType", Encoding.UTF8.GetBytes(eventType) },
                        { "Timestamp", Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("o")) }
                    },
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                _logger.LogInformation("Intentando publicar evento {EventType} con ID {Id} en topic {Topic}", 
                    eventType, id, _topic);

                _producer.Produce(_topic, message, deliveryReport =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        _logger.LogError("Fallo al entregar mensaje: {Reason}. Key: {Key}, Error Code: {ErrorCode}", 
                            deliveryReport.Error.Reason, id, deliveryReport.Error.Code);
                        
                        _publishedEvents.TryRemove(id, out _);
                    
                        if (deliveryReport.Error.Code == ErrorCode.UnknownTopicOrPart)
                        {
                            _logger.LogWarning("El topic {Topic} no existe. Intentando crearlo...", _topic);
                            CreateTopicIfNotExists(_topic, _producer.Name);
                            tcs.SetException(new KafkaException(deliveryReport.Error));
                        }
                        else
                        {
                            tcs.SetException(new KafkaException(deliveryReport.Error));
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Evento {EventType} publicado exitosamente. Key: {Key}, Partition: {Partition}, Offset: {Offset}", 
                            eventType, id, deliveryReport.Partition, deliveryReport.Offset);
                        tcs.SetResult(true);
                    }
                });
                
                // Flush inmediato para asegurar que el mensaje se intente enviar
                _producer.Flush(TimeSpan.FromMilliseconds(100));
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
