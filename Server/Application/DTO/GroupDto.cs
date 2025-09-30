using System;

namespace Server.Application.DTO
{
    public record class GroupDto(
        Guid id,
        string name,
        string? description,
        string? avatarPath
    );
}
