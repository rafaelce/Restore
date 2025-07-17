using System.Text.Json;
using API.Events;
using API.RequestHelpers;
using API.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;


namespace API.Services;

public class KafkaService : IKafkaService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaService(ILogger<KafkaService> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;

        // Usando o mapper
        var config = _kafkaSettings.ToProducerConfig();
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishUserEventAsync(UserEvent userEvent)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = userEvent.UserId,
                Value = JsonSerializer.Serialize(userEvent)
            };

            var result = await _producer.ProduceAsync(_kafkaSettings.UserEventsTopic, message);
            _logger.LogInformation("User event published to Kafka: {Topic} - Partition: {Partition} - Offset: {Offset}",
                result.Topic, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing user event to Kafka");
            throw;
        }
    }

    public async Task PublishOrderEventAsync(OrderEvent orderEvent)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = orderEvent.OrderId,
                Value = JsonSerializer.Serialize(orderEvent)
            };

            var result = await _producer.ProduceAsync(_kafkaSettings.OrderEventsTopic, message);
            _logger.LogInformation("Order event published to Kafka: {Topic} - Partition: {Partition} - Offset: {Offset}",
                result.Topic, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing order event to Kafka");
            throw;
        }
    }

    public async Task PublishSearchEventAsync(SearchEvent searchEvent)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = searchEvent.UserId,
                Value = JsonSerializer.Serialize(searchEvent)
            };

            var result = await _producer.ProduceAsync(_kafkaSettings.SearchEventsTopic, message);
            _logger.LogInformation("Search event published to Kafka: {Topic} - Partition: {Partition} - Offset: {Offset}",
                result.Topic, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing search event to Kafka");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
