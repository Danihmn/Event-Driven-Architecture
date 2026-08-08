using Confluent.Kafka;

namespace Contracts.Consumer;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create(string groupId);
}