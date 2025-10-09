namespace UserService.Core.Events;

public class UserUpdatedEvent : BaseEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
