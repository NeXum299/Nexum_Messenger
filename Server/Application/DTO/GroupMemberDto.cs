using System;

namespace Server.Application.DTO
{
    public record class GroupMemberDto(
        string firstName,
        string lastName,
        string userName,
        string? phoneNumber,
        string roleInGroup,
        DateTime joinedAt
    );
}
