namespace UserSerivce.Application.DTO.Created;

public record UserCreatedRequest(
    Guid Id,
    string UserName,
    string FirstName,
    string? LastName,
    string? MiddleName,
    string PhoneNumber,
    string AvatarPath,
    DateOnly? BirthDate,
    DateTime CreatedAt
);
