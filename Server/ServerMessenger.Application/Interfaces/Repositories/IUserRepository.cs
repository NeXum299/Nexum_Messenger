using System;
using System.Threading.Tasks;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Interface.Repositories
{
    /// <summary>Интерфейс репозитория для работы с пользователями.</summary>
    public interface IUserRepository
    {
        /// <summary>Получает пользователя по его идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        Task<Result<User?>> GetUserByIdAsync(Guid id);

        /// <summary>Получает пользователя по его никнейму.</summary>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <returns>Возвращает найденого пользователь.</returns>
        Task<Result<User?>> GetUserByUserNameAsync(string userName);

        /// <summary>Получает пользователя по номеру телефона.</summary>
        /// <param name="phoneNumber">Номер телефона пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        Task<Result<User?>> GetUserByPhoneNumberAsync(string? phoneNumber);

        /// <summary>Создает нового пользователя.</summary>
        /// <param name="user">Данные пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> CreateUserAsync(User user, string password);

        /// <summary>Обновляет данные пользователя.</summary>
        /// <param name="user">Обновленные данные пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> UpdateUserAsync(User user);

        /// <summary>Удаляет пользователя по идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> DeleteUserAsync(Guid id);
    }
}
