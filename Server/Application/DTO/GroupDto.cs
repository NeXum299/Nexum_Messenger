using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="avatarPath"></param>
    public record class GroupDto(
        Guid id,
        string name,
        string? description,
        string? avatarPath
    );
}
