using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Core.Entities
{
    /// <summary>Модель пользователя системы.</summary>
    public class UserEntity
    {
        /// <summary>Уникальный идентификатор пользователя.</summary>
        [Key] public Guid Id { get; set; }

        /// <summary>Имя пользователя.</summary>
        public string FirstName { get; set; }= string.Empty;

        /// <summary>Фамилия пользователя.</summary>
        public string LastName { get; set; }= string.Empty;

        /// <summary>Никнейм пользователя</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Номер телефона пользователя.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Путь к аватару пользователя.</summary>
        public string? AvatarPath { get; set; }

        /// <summary>Дата рождения пользователя.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>Дата и время создания учетной записи.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Хэш пароля пользователя.</summary>
        public string? PasswordHash { get; set; }

        /// <summary>Роль пользователя в системе.</summary>
        public string? Role { get; set; } = "User";

        /// <summary>Refresh token для обновления access token.</summary>
        public string? RefreshToken { get; set; }

        /// <summary>Время истечения срока действия refresh token.</summary>
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>Список, в каких группах есть пользователь.</summary>
        public ICollection<GroupMemberEntity> GroupMemberships { get; set; } = new List<GroupMemberEntity>();

        /// <summary>Список друзей пользователя.</summary>
        public ICollection<FriendEntity> Friends { get; set; } = new List<FriendEntity>();
    }
}
