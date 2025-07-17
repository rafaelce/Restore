using System.Text.Json;
using API.Events;
using API.RequestHelpers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace API.Consumers.Kafka;

public class SearchEventsConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<SearchEventsConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public SearchEventsConsumer(ILogger<SearchEventsConsumer> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;

        var config = _kafkaSettings.ToConsumerConfig(_kafkaSettings.SearchEventsGroupId);
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SearchEventsConsumer starting...");

        _consumer.Subscribe(_kafkaSettings.SearchEventsTopic);
        _logger.LogInformation("SearchEventsConsumer subscribed to topic: {Topic}", _kafkaSettings.SearchEventsTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("SearchEventsConsumer waiting for message...");

                    // Use Task.Run para não bloquear a thread principal
                    var result = await Task.Run(() => _consumer.Consume(TimeSpan.FromSeconds(5)), stoppingToken);

                    if (result != null)
                    {
                        _logger.LogInformation("SearchEventsConsumer received message");
                        var searchEvent = JsonSerializer.Deserialize<SearchEvent>(result.Message.Value);

                        if (searchEvent != null)
                        {
                            await ProcessSearchEventAsync(searchEvent);
                        }

                        _consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("SearchEventsConsumer timeout, continuing...");
                    // Aguarde um pouco antes de tentar novamente
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming search event");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in SearchEventsConsumer");
                    await Task.Delay(5000, stoppingToken); // Aguarde 5 segundos antes de tentar novamente
                }
            }
        }
        finally
        {
            _logger.LogInformation("SearchEventsConsumer stopping...");
            _consumer.Close();
        }
    }

    private async Task ProcessSearchEventAsync(SearchEvent searchEvent)
    {
        _logger.LogInformation("Processing search event: {UserId} - {SearchTerm} - {ResultsCount}",
            searchEvent.UserId, searchEvent.SearchTerm, searchEvent.ResultsCount);

        await Task.Delay(100); // Simula processamento
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
