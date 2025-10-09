namespace UserService.Application.DTO.Received;

public record UserReceivedRequest(
    Guid Id,
    string UserName,
    string FirstName,
    string? LastName,
    string? MiddleName,
    string PhoneNumber,
    string AvatarPath,
    DateOnly? BirthDate,
    DateTime CreatedAt,
    string PasswordHash
);
