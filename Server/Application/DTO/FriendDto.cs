using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="avatarPath"></param>
    public record class FriendDto(
        Guid id,
        string userName,
        string firstName,
        string lastName,
        string? avatarPath
    );
}
