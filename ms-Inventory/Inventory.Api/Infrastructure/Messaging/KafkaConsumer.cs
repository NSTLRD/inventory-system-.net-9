
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Inventory.Api.Application.Events;
using Inventory.Api.Application.Events.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Inventory.Api.Infrastructure.Messaging
{
    /// <summary>
    /// Lee del topic Kafka y publica los IntegrationEvents en el bus interno.
    /// </summary>
    public class KafkaConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _isRunning = false;
        private Task? _executingTask;
        private CancellationTokenSource? _stoppingCts;

        // Diccionario para mapear el nombre del evento
        private static readonly Dictionary<string, Type> _eventTypes = new()
        {
            { "ProductCreatedEvent", typeof(ProductCreatedIntegrationEvent) },
            { "ProductUpdatedEvent", typeof(ProductUpdatedIntegrationEvent) },
            { "ProductDeletedEvent", typeof(ProductDeletedIntegrationEvent) }
        };

        public KafkaConsumer(
            IConfiguration configuration,
            IMediator mediator,
            IServiceProvider serviceProvider,
            ILogger<KafkaConsumer> logger)
        {
            _configuration = configuration;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _logger = logger;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Kafka consumer service");
            
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = Task.Factory.StartNew(() => ExecuteAsync(_stoppingCts.Token), 
                _stoppingCts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Kafka consumer service");

            if (_executingTask == null)
            {
                return;
            }

            try
            {

                _stoppingCts?.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(5000, cancellationToken));
            }
        }

       
        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka consumer is running");
            _isRunning = true;

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["KafkaSettings:BootstrapServers"],
                GroupId = _configuration["KafkaSettings:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                MaxPollIntervalMs = 300000,        // 5 minutos
                SessionTimeoutMs = 30000,          // 30 segundos
                HeartbeatIntervalMs = 10000,       // 10 segundos
                EnablePartitionEof = true,         // Recibir EndOfPartition
                EnableAutoOffsetStore = false      
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => 
                    _logger.LogWarning("Error de Kafka Consumer: {Reason} ({Code})", e.Reason, e.Code))
                .SetPartitionsAssignedHandler((c, partitions) => 
                    _logger.LogInformation("Asignadas particiones: {Partitions}", 
                        string.Join(", ", partitions.Select(p => p.Partition.Value))))
                .Build();
            
            try
            {
                // Suscribirse específicamente al topic de producto
                var topicToConsume = _configuration["KafkaSettings:ConsumerTopic"] ?? "product-events";
                _logger.LogInformation("Intentando suscribirse al tema {Topic}", topicToConsume);
                
                try 
                {
                    consumer.Subscribe(topicToConsume);
                    _logger.LogInformation("Suscripción exitosa al tema {Topic}", topicToConsume);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al suscribirse al tema {Topic}. Intentando continuar...", topicToConsume);
                }

                int consecutiveErrors = 0;
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));
                        
                        if (consumeResult == null || consumeResult.IsPartitionEOF)
                        {
                            continue;
                        }
                        
                        if (!string.IsNullOrWhiteSpace(consumeResult.Message.Value))
                        {
                            _logger.LogInformation("Mensaje recibido: Partition: {Partition}, Offset: {Offset}, Timestamp: {Timestamp}",
                                consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Timestamp);
                                
                            await ProcessMessageAsync(consumeResult.Message.Value);
                            
                            // Almacenar el offset manualmente
                            consumer.StoreOffset(consumeResult);
                            
                            // Resetear contador de errores consecutivos
                            consecutiveErrors = 0;
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        consecutiveErrors++;
                        _logger.LogError(ex, "Error consumiendo mensaje (intento {Count}): {Error}", 
                            consecutiveErrors, ex.Error.Reason);
                            
                        if (consecutiveErrors > 5)
                        {
                            _logger.LogWarning("Demasiados errores consecutivos, esperando 10 segundos...");
                            await Task.Delay(10000, stoppingToken);
                            
                            // Reintentar suscripción
                            try 
                            {
                                consumer.Unsubscribe();
                                await Task.Delay(1000, stoppingToken);
                                consumer.Subscribe(topicToConsume);
                                _logger.LogInformation("Re-suscripción exitosa al tema {Topic}", topicToConsume);
                                consecutiveErrors = 0;
                            }
                            catch (Exception subEx)
                            {
                                _logger.LogError(subEx, "Error al re-suscribirse al tema {Topic}", topicToConsume);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
                _logger.LogInformation("Kafka consumer was stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Kafka consumer");
            }
            finally
            {
                consumer.Close();
                _isRunning = false;
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                _logger.LogInformation("Processing message: {Message}", message);
                
                // Deserializar el mensaje para determinar el tipo de evento
                using var doc = JsonDocument.Parse(message);
                
                // Intenta determinar el tipo de evento basado en el contenido
                Type? eventType = null;
                foreach (var kvp in _eventTypes)
                {
                    if (message.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        eventType = kvp.Value;
                        break;
                    }
                }
                
                if (eventType != null)
                {
                    // Deserializa el mensaje al tipo de evento específico
                    var @event = JsonSerializer.Deserialize(message, eventType, _jsonOptions);
                    if (@event != null)
                    {
                        _logger.LogInformation("Publicando evento al bus interno: {EventType}", eventType.Name);
                        await _mediator.Send(@event);
                    }
                }
                else
                {
                    _logger.LogWarning("No se pudo determinar el tipo de evento para el mensaje: {Message}", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }

        public bool IsRunning => _isRunning;
    }
}
