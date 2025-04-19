using Confluent.Kafka;
using System.Text.Json;

namespace SkillSync.UserService.Infrastructure.Messaging;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message);
}

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer()
    {
        var config = new ProducerConfig{BootstrapServers = "localhost:9092"};
        _producer = new ProducerBuilder<Null, string>(config).Build();

    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string>{Value = json});
    }
}