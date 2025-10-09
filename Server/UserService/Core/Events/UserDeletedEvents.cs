namespace UserService.Core.Events;

public class UserDeletedEvent : BaseEvent
{
    public Guid Id { get; set; }
}
