using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="avatarPath"></param>
    /// <param name="status"></param>
    /// <param name="createdAt"></param>
    public record class FriendRequestDto(
        Guid id,
        Guid userId,
        string userName,
        string firstName,
        string lastName,
        string? avatarPath,
        string status,
        DateTime createdAt
    );
}
