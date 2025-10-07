namespace UserSerivce.Application.DTO.Updated;

public record UserUpdatedResponse(
    string UserName,
    string FirstName,
    string? LastName,
    string? MiddleName,
    string PhoneNumber,
    string AvatarPath,
    DateOnly? BirthDate,
    DateTime CreatedAt
);
