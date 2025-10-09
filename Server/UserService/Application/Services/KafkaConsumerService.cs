using Confluent.Kafka;

namespace UserService.Application.Services;

public class KafkaConsumerService
{
    private readonly IConsumer<Null, string> _consumer;

    public KafkaConsumerService()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9093",
            GroupId = "my-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void ConsumeMessages(string topic)
    {
        _consumer.Subscribe(topic);

        try
        {
            while (true)
            {
                var consumeResult = _consumer.Consume();
                Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
            }
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error consuming message: {e.Error.Reason}");
        }
    }
}
