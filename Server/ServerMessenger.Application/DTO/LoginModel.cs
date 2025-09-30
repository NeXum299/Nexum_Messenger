using System.ComponentModel.DataAnnotations;

namespace ServerMessenger.Application.DTO
{
    /// <summary>Модель для входа пользователя в систему.</summary>
    public class LoginModel
    {
        /// <summary>Номер телефона пользователя.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Пароль пользователя.</summary>
        public string? Password { get; set; }
    }
}
