using System;
using System.IO;
using System.Threading.Tasks;
using Server.Application.Interface.Repositories;
using Server.Application.Results;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Server.Core.Entities;
using Server.Application.Interface.Services;
using System.Linq;
using Server.Application.Validators;

namespace Server.Application.Services
{
    /// <summary>Сервис для работы с пользователями.</summary>
    public class UserService : IUserService
    {
        private readonly UserValidator _userValidator;
        private readonly IUserRepository _userRepositories;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserService> _logger;

        /// <summary>Инициализирует новый экземпляр класса UserService.</summary>
        /// <param name="userRepositories">Репозиторий пользователей.</param>
        /// <param name="environment">Окружение веб-хоста.</param>
        /// <param name="userValidator">Валидатор для класса <see cref="User"/>.</param>
        /// <param name="logger">Логгер.</param>
        public UserService(IUserRepository userRepositories, IWebHostEnvironment environment,
            ILogger<UserService> logger, UserValidator userValidator)
        {
            _userRepositories = userRepositories;
            _environment = environment;
            _logger = logger;
            _userValidator = userValidator;
        }

        /// <summary>Создает нового пользователя в системе.</summary>
        /// <param name="user">Объект пользователя с данными для создания.</param>
        /// <param name="password">Пароль пользователя в открытом виде.</param>
        /// <returns>Результат выполнения операции: успех или ошибка.</returns>
        public async Task<Result> CreateUserAsync(User user, string password)
        {
            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var result = await _userRepositories.CreateUserAsync(user, password);
            return result.Fail ? Result.Error("User creation Error") : Result.Ok();
        }

        /// <summary>Обновляет данные пользователя.</summary>
        /// <param name="user">Объект пользователя с обновленными данными.</param>
        /// <returns>Результат выполнения операции: успех или ошибка.</returns>
        public async Task<Result> UpdateUserAsync(User user)
        {
            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var result = await _userRepositories.UpdateUserAsync(user);
            return result.Fail ? Result.Error("User update error") : Result.Ok();
        }

        /// <summary>Обновляет аватар пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="avatarPath">Путь к новому файлу аватара.</param>
        /// <returns>Результат выполнения операции: успех или ошибка.</returns>
        /// <remarks>Удаляет старый аватар пользователя (если он не является дефолтным) перед сохранением нового.</remarks>
        public async Task<Result> UpdateUserAvatarAsync(Guid userId, string avatarPath)
        {
            var userResult = await _userRepositories.GetUserByIdAsync(userId);

            if (userResult.Fail || userResult.Value == null)
                return Result.Error("User not found");

            var user = userResult.Value;

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            if (!string.IsNullOrEmpty(user.AvatarPath) &&
                !user.AvatarPath.Equals("/avatars/users/default.jpg"))
            {
                try
                {
                    var oldAvatarPath = Path.Combine(_environment.WebRootPath,
                        user.AvatarPath.TrimStart('/'));

                    if (File.Exists(oldAvatarPath))
                        File.Delete(oldAvatarPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting old avatar");
                    return Result.Error("Error deleting old avatar");
                }
            }

            user.AvatarPath = avatarPath;

            var updateResult = await _userRepositories.UpdateUserAsync(user);
            return updateResult.Fail ? Result.Error("Error updating the user's avatar") 
                : Result.Ok();
        }

        /// <summary>Удаляет пользователя по указанному идентификатору.</summary>
        /// <param name="id">Уникальный идентификатор пользователя.</param>
        /// <returns>Результат выполнения операции: успех или ошибка.</returns>
        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var user = await _userRepositories.GetUserByIdAsync(id);
            if (user.Fail || user.Value == null)
                return Result.Error("User deletion error or him is null");

            var validationResult = await _userValidator.ValidateAsync(user.Value);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var result = await _userRepositories.DeleteUserAsync(id);
            return result.Fail ? Result.Error("User deletion error") : Result.Ok(); 
        }

        /// <summary>Получает пользователя по его никнейму.</summary>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <returns>Возвращает найденого пользователь.</returns>
        public async Task<Result<User?>> GetUserByUserNameAsync(string userName)
        {
            var userResult = await _userRepositories.GetUserByUserNameAsync(userName);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error<User?>(null, "Error get the user or he is null");

            return Result.Ok<User?>(userResult.Value);
        }

        /// <summary>Находит пользователя по его уникальному идентификатору.</summary>
        /// <param name="id">Уникальный идентификатор пользователя.</param>
        /// <returns>Результат выполнения операции с найденным пользователем или ошибкой.</returns>
        public async Task<Result<User?>> GetUserByIdAsync(Guid id)
        {
            var userResult = await _userRepositories.GetUserByIdAsync(id);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error<User?>(null, "Error get user or he is null");

            return Result.Ok<User?>(userResult.Value);
        }

        /// <summary>Находит пользователя по номеру телефона.</summary>
        /// <param name="phoneNumber">Номер телефона пользователя.</param>
        /// <returns>Результат выполнения операции с найденным пользователем или ошибкой.</returns>
        public async Task<Result<User?>> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var userResult = await _userRepositories.GetUserByPhoneNumberAsync(phoneNumber);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error<User?>(null, "Error get user or him is null");

            return Result.Ok<User?  >(userResult.Value);
        }

        /// <summary>Обновляет refresh-токен пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="refreshToken">Новый refresh-токен.</param>
        /// <param name="expiryTime">Дата и время истечения срока действия токена.</param>
        /// <returns>Результат выполнения операции: успех или ошибка.</returns>
        public async Task<Result> UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime)
        {
            var userResult = await _userRepositories.GetUserByIdAsync(userId);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error("Refresh token update error");

            var user = userResult.Value;

            var validationResult = await _userValidator.ValidateAsync(user);
            if (validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;

            var updateResult = await _userRepositories.UpdateUserAsync(user);

            return updateResult.Fail ? Result.Error("Refresh token update error")
                : Result.Ok();
        }
    }
}
