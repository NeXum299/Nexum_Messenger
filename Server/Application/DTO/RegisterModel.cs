using System;

namespace Server.Application.DTO
{
    public record class RegisterModel(
        string firtsName,
        string lastName,
        string userName,
        string? phoneNumber,
        string? avatarPath,
        DateOnly? BirthDate,
        string password
    );
}
