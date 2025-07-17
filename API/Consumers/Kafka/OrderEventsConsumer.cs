using System.Text.Json;
using API.Events;
using API.RequestHelpers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace API.Consumers.Kafka;

public class OrderEventsConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<OrderEventsConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public OrderEventsConsumer(ILogger<OrderEventsConsumer> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;

        var config = _kafkaSettings.ToConsumerConfig(_kafkaSettings.OrderEventsGroupId);
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderEventsConsumer starting...");

        _consumer.Subscribe(_kafkaSettings.OrderEventsTopic);
        _logger.LogInformation("OrderEventsConsumer subscribed to topic: {Topic}", _kafkaSettings.OrderEventsTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("OrderEventsConsumer waiting for message...");

                    // Use Task.Run para não bloquear a thread principal
                    var result = await Task.Run(() => _consumer.Consume(TimeSpan.FromSeconds(5)), stoppingToken);

                    if (result != null)
                    {
                        _logger.LogInformation("OrderEventsConsumer received message");
                        var orderEvent = JsonSerializer.Deserialize<OrderEvent>(result.Message.Value);

                        if (orderEvent != null)
                        {
                            await ProcessOrderEventAsync(orderEvent);
                        }

                        _consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("OrderEventsConsumer timeout, continuing...");
                    // Aguarde um pouco antes de tentar novamente
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming order event");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in OrderEventsConsumer");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
            }
        }
        finally
        {
            _logger.LogInformation("OrderEventsConsumer stopping...");
            _consumer.Close();
        }
    }

    private async Task ProcessOrderEventAsync(OrderEvent orderEvent)
    {
        _logger.LogInformation("Processing order event: {OrderId} - {EventType} - {TotalAmount}",
            orderEvent.OrderId, orderEvent.EventType, orderEvent.TotalAmount);

        await Task.Delay(100); // Simula processamento
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}