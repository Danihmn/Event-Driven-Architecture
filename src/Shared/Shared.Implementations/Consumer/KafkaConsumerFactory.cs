using Confluent.Kafka;
using Contracts.Consumer;
using Microsoft.Extensions.Configuration;

namespace Shared.Implementations.Consumer;

public class KafkaConsumerFactory(IConfiguration configuration) : IKafkaConsumerFactory
{
    public IConsumer<string, string> Create(string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration.GetConnectionString("kafka"),
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };

        return new ConsumerBuilder<string, string>(config).Build();
    }
}