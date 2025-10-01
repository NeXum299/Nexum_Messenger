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
    /// <param name="roleInGroup"></param>
    /// <param name="joinedAt"></param>
    public record class GroupMemberDto(
        string firstName,
        string lastName,
        string userName,
        string? phoneNumber,
        string roleInGroup,
        DateTime joinedAt
    );
}
