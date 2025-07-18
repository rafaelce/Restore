using Confluent.Kafka;

namespace API.RequestHelpers;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string UserEventsTopic { get; set; } = string.Empty;
    public string OrderEventsTopic { get; set; } = string.Empty;
    public string SearchEventsTopic { get; set; } = string.Empty;
    public int MessageSendMaxRetries { get; set; } = 3;
    public int RetryBackoffMs { get; set; } = 1000;
    public bool EnableIdempotence { get; set; } = true;

    // Configurações dos consumidores
    public string UserEventsGroupId { get; set; } = string.Empty;
    public string OrderEventsGroupId { get; set; } = string.Empty;
    public string SearchEventsGroupId { get; set; } = string.Empty;
    public string AutoOffsetReset { get; set; } = "Earliest";
    public bool EnableAutoCommit { get; set; } = false;

    public ProducerConfig ToProducerConfig()
    {
        return new ProducerConfig
        {
            BootstrapServers = BootstrapServers,
            ClientId = ClientId,
            EnableIdempotence = EnableIdempotence,
            Acks = Acks.All,
            MessageSendMaxRetries = MessageSendMaxRetries,
            RetryBackoffMs = RetryBackoffMs
        };
    }

    public ConsumerConfig ToConsumerConfig(string groupId)
    {
        return new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(AutoOffsetReset),
            EnableAutoCommit = EnableAutoCommit
        };
    }
}
