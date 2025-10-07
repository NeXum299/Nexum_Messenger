namespace UserSerivce.Application.DTO.Updated;

public record UserUpdatedRequest(
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
