using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Core.Entities;
using Server.Infrastructure.Database;

namespace Server.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordHasher"></param>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        public UserRepository(IPasswordHasher<UserEntity> passwordHasher,
            DatabaseContext dbContext, ILogger<UserRepository> logger)
        {
            _passwordHasher = passwordHasher;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task CreateUserAsync(UserEntity user, string password)
        {
            try
            {
                // Нужно добавить проверку на уже существующего пользователя
                user.PasswordHash = _passwordHasher
                    .HashPassword(user, password);
                user.CreatedAt = DateTime.UtcNow;
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                throw new UserCreationException("Ошибка обновления базы данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new UserCreationException(
                    "Неожиданная ошибка при создании пользователя");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            try
            {
                return await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception(
                    "Неожиданная ошибка при извлечении пользователя");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception(
                    "Неожиданная ошибка при извлечении пользователя");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByPhoneNumberAsync(string? phoneNumber)
        {
            try
            {
                return await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception(
                    "Неожиданная ошибка при извлечении пользователя");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(UserEntity user)
        {
            try
            {
                // Нужно добавить проверку существует ли вообще такой пользователь перед обновлением данных
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception(
                    "Неожиданная ошибка во время обновления пользователя");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    throw new UserNotFoundException("User not found");

                _dbContext.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception("Неожиданная ошибка во время удаления пользователя");
            }
        }
    }
}
