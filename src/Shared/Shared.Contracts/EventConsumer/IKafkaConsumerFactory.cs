using Confluent.Kafka;

namespace Contracts.EventConsumer;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create(string groupId);
}