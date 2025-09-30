using System;

namespace ServerMessenger.Application.DTO
{
    /// <summary>Модель для регистрации нового пользователя.</summary>
    public class RegisterModel
    {
        /// <summary>Имя пользователя.</summary>
        public string? FirstName { get; set; }

        /// <summary>Фамилия пользователя.</summary>
        public string? LastName { get; set; }

        /// <summary>Никнейм пользователя.</summary>
        public string UserName { get; set; } = null!;

        /// <summary>Номер телефона пользователя.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Путь к аватару пользователя (по умолчанию '/avatars/users/default.jpg').</summary>
        public string? AvatarPath { get; set; } = "/avatars/users/default.jpg";

        /// <summary>Дата рождения пользователя (может быть null).</summary>
        public DateOnly? BirthDate { get; set; } = null;

        /// <summary>Пароль пользователя.</summary>
        public string? Password { get; set; }
    }
}
