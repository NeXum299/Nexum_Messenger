using System;
using System.Threading.Tasks;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>Интерфейс репозитория для работы с пользователями.</summary>
    public interface IUserRepository
    {
        /// <summary>Получает пользователя по его идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        Task<UserEntity?> GetUserByIdAsync(Guid id);

        /// <summary>Получает пользователя по его никнейму.</summary>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <returns>Возвращает найденого пользователь.</returns>
        Task<UserEntity?> GetUserByUserNameAsync(string userName);

        /// <summary>Получает пользователя по номеру телефона.</summary>
        /// <param name="phoneNumber">Номер телефона пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        Task<UserEntity?> GetUserByPhoneNumberAsync(string? phoneNumber);

        /// <summary>Создает нового пользователя.</summary>
        /// <param name="user">Данные пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task CreateUserAsync(UserEntity user, string password);

        /// <summary>Обновляет данные пользователя.</summary>
        /// <param name="user">Обновленные данные пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task UpdateUserAsync(UserEntity user);

        /// <summary>Удаляет пользователя по идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task DeleteUserAsync(Guid id);
    }
}
