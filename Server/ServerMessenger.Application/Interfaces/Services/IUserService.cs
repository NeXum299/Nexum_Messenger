using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Interface.Services
{
    /// <summary>Интерфейс сервиса для работы с пользователями.</summary>
    public interface IUserService
    {
        /// <summary>Создает нового пользователя.</summary>
        /// <param name="user">Данные пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> CreateUserAsync(User user, string password);

        /// <summary>Получает пользователя по его никнейму.</summary>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <returns>Возвращает найденого пользователь.</returns>
        Task<Result<User?>> GetUserByUserNameAsync(string userName);

        /// <summary>Получает пользователя по идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции с данными пользователя.</returns>
        Task<Result<User?>> GetUserByIdAsync(Guid id);

        /// <summary>Получает пользователя по номеру телефона.</summary>
        /// <param name="phoneNumber">Номер телефона пользователя.</param>
        /// <returns>Результат операции с данными пользователя.</returns>
        Task<Result<User?>> GetUserByPhoneNumberAsync(string phoneNumber);

        /// <summary>Обновляет данные пользователя.</summary>
        /// <param name="user">Обновленные данные пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> UpdateUserAsync(User user);

        /// <summary>Обновляет аватар пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="avatarPath">Путь к новому аватару.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> UpdateUserAvatarAsync(Guid userId, string avatarPath);

        /// <summary>Удаляет пользователя.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> DeleteUserAsync(Guid id);

        /// <summary>Обновляет refresh-токен пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="refreshToken">Новый refresh-токен.</param>
        /// <param name="expiryTime">Время истечения токена.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime);
    }
}
