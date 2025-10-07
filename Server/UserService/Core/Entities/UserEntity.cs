namespace UserSerivce.Core.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string AvatarPath { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
}
