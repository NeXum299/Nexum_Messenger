using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerMessenger.Application.Interface.Repositories;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;
using ServerMessenger.Infrastructure.Database;

namespace ServerMessenger.Infrastructure.Repositories
{
    /// <summary>Реализация репозитория для работы с пользователями.</summary>
    public class UserRepository : IUserRepository
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        /// <summary>Инициализирует новый экземпляр репозитория пользователей.</summary>
        /// <param name="passwordHasher">Сервис хеширования паролей.</param>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="logger">Логгер.</param>
        public UserRepository(IPasswordHasher<User> passwordHasher, DatabaseContext dbContext, ILogger<UserRepository> logger)
        {
            _passwordHasher = passwordHasher;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>Создает нового пользователя с указанным паролем.</summary>
        /// <param name="user">Данные пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> CreateUserAsync(User user, string password)
        {
            try
            {
                // Нужно добавить проверку на уже существующего пользователя
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                user.CreatedAt = DateTime.UtcNow;
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                return Result.Error("Database update error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error($"Unexpected error");
            }
        }

        /// <summary>Получает пользователя по его никнейму.</summary>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <returns>Возвращает найденого пользователь.</returns>
        public async Task<Result<User?>> GetUserByUserNameAsync(string userName)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error<User?>(null, $"Unexpected error.");
            }
        }

        /// <summary>Получает пользователя по его идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        public async Task<Result<User?>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error<User?>(null, $"Unexpected error.");
            }
        }

        /// <summary>Получает пользователя по номеру телефона.</summary>
        /// <param name="phoneNumber">Номер телефона пользователя.</param>
        /// <returns>Результат операции, содержащий пользователя или null.</returns>
        public async Task<Result<User?>> GetUserByPhoneNumberAsync(string? phoneNumber)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error<User?>(null, $"Unexpected error.");
            }
        }

        /// <summary>Обновляет данные пользователя.</summary>
        /// <param name="user">Обновленные данные пользователя.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> UpdateUserAsync(User user)
        {
            try
            {
                // Нужно добавить проверку существует ли вообще такой пользователь перед обновлением данных
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                return Result.Error($"Database update error.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error($"Unexpected error.");
            }
        }

        /// <summary>Удаляет пользователя по идентификатору.</summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return Result.Error("User not found");

                _dbContext.Remove(user);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                return Result.Error("Database update error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error($"Unexpected error: {ex.Message}");
            }
        }
    }
}
