using System.Text.Json;
using API.Events;
using API.RequestHelpers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace API.Consumers.Kafka;

public class UserEventsConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<UserEventsConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public UserEventsConsumer(ILogger<UserEventsConsumer> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;

        _logger.LogInformation("Initializing UserEventsConsumer...");

        var config = _kafkaSettings.ToConsumerConfig(_kafkaSettings.UserEventsGroupId);
        _logger.LogInformation("Kafka config created: {BootstrapServers}", config.BootstrapServers);

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _logger.LogInformation("UserEventsConsumer initialized successfully");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UserEventsConsumer starting...");

        _consumer.Subscribe(_kafkaSettings.UserEventsTopic);
        _logger.LogInformation("UserEventsConsumer subscribed to topic: {Topic}", _kafkaSettings.UserEventsTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("UserEventsConsumer waiting for message...");

                    // Use Task.Run para não bloquear a thread principal
                    var result = await Task.Run(() => _consumer.Consume(TimeSpan.FromSeconds(5)), stoppingToken);

                    if (result != null)
                    {
                        _logger.LogInformation("UserEventsConsumer received message");
                        var userEvent = JsonSerializer.Deserialize<UserEvent>(result.Message.Value);

                        if (userEvent != null)
                        {
                            await ProcessUserEventAsync(userEvent);
                        }

                        _consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("UserEventsConsumer timeout, continuing...");
                    // Aguarde um pouco antes de tentar novamente
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming user event");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in UserEventsConsumer");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
            }
        }
        finally
        {
            _logger.LogInformation("UserEventsConsumer stopping...");
            _consumer.Close();
        }
    }

    private async Task ProcessUserEventAsync(UserEvent userEvent)
    {
        _logger.LogInformation("Processing user event: {UserId} - {EventType} - {ProductId}",
            userEvent.UserId, userEvent.EventType, userEvent.ProductId);

        await Task.Delay(100); // Simula processamento
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
