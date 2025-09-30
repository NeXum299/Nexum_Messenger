using System;

namespace Server.Application.DTO
{
    /// <summary>
    /// DTO друга
    /// </summary>
    /// <param name="id">Идентификатор друга.</param>
    /// <param name="userName">Никнейм друга.</param>
    /// <param name="firstName">Имя друга.</param>
    /// <param name="lastName">Фамилия друга.</param>
    /// <param name="avatarPath">Аватар друга.</param>
    public record class FriendDto(
        Guid id,
        string userName,
        string firstName,
        string lastName,
        string? avatarPath
    );
}
