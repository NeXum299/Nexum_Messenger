namespace UserService.Core.Events;

public abstract class BaseEvent
{
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0";
}
