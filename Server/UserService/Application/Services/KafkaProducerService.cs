using System.Text.Json;
using Confluent.Kafka;
using UserService.Application.Interfaces.Services;

namespace UserService.Application.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "kafka:9093"
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task SendMessageAsync(string topic, string message)
    {
        try
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            Console.WriteLine($"Message '{message}' sent to topic '{topic}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to Kafka: {ex.Message}");
            throw;
        }
    }
    
    public async Task SendEventAsync<T>(string topic, T eventObject)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(eventObject, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
            Console.WriteLine($"Event of type {typeof(T).Name} sent to topic '{topic}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending event to Kafka: {ex.Message}");
            throw;
        }
    }
}
