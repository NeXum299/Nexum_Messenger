using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServerMessenger.Core.Entities
{
    /// <summary>Модель пользователя системы.</summary>
    public class User
    {
        /// <summary>Уникальный идентификатор пользователя.</summary>
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Имя пользователя.</summary>
        public string? FirstName { get; set; }

        /// <summary>Фамилия пользователя.</summary>
        public string? LastName { get; set; }

        /// <summary>Никнейм пользователя</summary>
        public string UserName { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Номер телефона пользователя.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Путь к аватару пользователя (по умолчанию "/avatars/users/default.jpg").</summary>
        public string AvatarPath { get; set; } = "/avatars/users/default.jpg";

        /// <summary>Дата рождения пользователя (может быть null).</summary>
        public DateOnly? BirthDate { get; set; } = null;

        /// <summary>Дата и время создания учетной записи.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Хэш пароля пользователя.</summary>
        public string? PasswordHash { get; set; }

        /// <summary>Роль пользователя в системе (по умолчанию "User").</summary>
        public string? Role { get; set; } = "User";

        /// <summary>Refresh token для обновления access token.</summary>
        public string? RefreshToken { get; set; }

        /// <summary>Время истечения срока действия refresh token.</summary>
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>Список, в каких группах есть пользователь (по умолчанию пустой список.)</summary>
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();

        /// <summary>Список друзей пользователя.</summary>
        public ICollection<Friend> Friends { get; set; } = new List<Friend>();
    }
}
