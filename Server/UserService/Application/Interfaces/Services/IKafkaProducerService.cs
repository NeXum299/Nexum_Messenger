namespace UserService.Application.Interfaces.Services;

public interface IKafkaProducerService
{
    Task SendMessageAsync(string topic, string message);
    Task SendEventAsync<T>(string topic, T eventObject);
}
