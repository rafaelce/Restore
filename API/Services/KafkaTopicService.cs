using API.RequestHelpers;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;

namespace API.Services;

public class KafkaTopicService
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaTopicService> _logger;

    public KafkaTopicService(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaTopicService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task CreateTopicsAsync()
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            var topics = new[]
            {
                    _kafkaSettings.UserEventsTopic,
                    _kafkaSettings.OrderEventsTopic,
                    _kafkaSettings.SearchEventsTopic
                };

            foreach (var topic in topics)
            {
                try
                {
                    var topicSpec = new TopicSpecification
                    {
                        Name = topic,
                        ReplicationFactor = 1,
                        NumPartitions = 3
                    };

                    await adminClient.CreateTopicsAsync(new[] { topicSpec });
                    _logger.LogInformation("Topic created: {Topic}", topic);
                }
                catch (CreateTopicsException ex) when (ex.Message.Contains("already exists"))
                {
                    _logger.LogInformation("Topic already exists: {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating topic: {Topic}", topic);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Kafka topics");
        }
    }
}
