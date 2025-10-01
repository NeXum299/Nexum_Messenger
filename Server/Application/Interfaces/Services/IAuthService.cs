using System.Threading.Tasks;
using Server.Application.DTO;

namespace Server.Application.Interface.Services
{
    /// <summary>Интерфейс сервиса аутентификации, предоставляющий методы для регистрации и входа пользователей.</summary>
    public interface IAuthService
    {
        /// <summary>Регистрирует нового пользователя асинхронно.</summary>
        /// <param name="model">Модель регистрации, содержащая данные пользователя.</param>
        /// <returns>Результат операции, содержащий информацию об успехе или ошибке.</returns>
        Task RegisterUserAsync(RegisterModel model);

        /// <summary>Выполняет вход пользователя асинхронно.</summary>
        /// <param name="model">Модель входа, содержащая учетные данные пользователя.</param>
        /// <returns>Результат операции, содержащий информацию об успехе или ошибке.</returns>
        Task LoginUserAsync(LoginModel model);
    }
}
