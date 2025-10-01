using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="userName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="avatarPath"></param>
    /// <param name="birthDate"></param>
    /// <param name="password"></param>
    public record class RegisterModel(
        string firstName,
        string lastName,
        string userName,
        string? phoneNumber,
        string? avatarPath,
        DateOnly? birthDate,
        string password
    );
}
