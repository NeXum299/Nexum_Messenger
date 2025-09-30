using System;

namespace Server.Application.DTO
{
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
