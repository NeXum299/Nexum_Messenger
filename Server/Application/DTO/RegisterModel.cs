using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firtsName"></param>
    /// <param name="lastName"></param>
    /// <param name="userName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="avatarPath"></param>
    /// <param name="BirthDate"></param>
    /// <param name="password"></param>
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
