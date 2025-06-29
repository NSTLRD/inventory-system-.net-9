// Inventory.Api/Infrastructure/Messaging/KafkaConsumer.cs
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

        // Diccionario para mapear el nombre del evento a su tipo
        private static readonly Dictionary<string, Type> _eventTypes = new()
        {
            { "ProductCreatedEvent", typeof(ProductCreatedIntegrationEvent) },
            { "ProductUpdatedEvent", typeof(ProductUpdatedIntegrationEvent) },
            { "ProductDeletedEvent", typeof(ProductDeletedIntegrationEvent) }
        };

        // En el constructor, inicializar el mediator
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
                // Signal cancellation to the executing method
                _stoppingCts?.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(5000, cancellationToken));
            }
        }

        // Este es un método privado, no una implementación de la interfaz
        private async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka consumer is running");
            _isRunning = true;

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "inventory-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            
            try
            {
                consumer.Subscribe(new[] { "product-events" });

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        if (consumeResult != null && !string.IsNullOrWhiteSpace(consumeResult.Message.Value))
                        {
                            await ProcessMessageAsync(consumeResult.Message.Value);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message");
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
                // Resto del método de procesamiento...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }
}
