namespace UserService.Application.DTO.Created;

public record UserCreatedRequest(
    Guid Id,
    string UserName,
    string FirstName,
    string? LastName,
    string? MiddleName,
    string PhoneNumber,
    string AvatarPath,
    string Password,
    DateOnly? BirthDate,
    DateTime CreatedAt
);
