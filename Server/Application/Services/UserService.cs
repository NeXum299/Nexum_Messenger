using System;
using System.IO;
using System.Threading.Tasks;
using Server.Application.Interface.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Server.Core.Entities;
using Server.Application.Interface.Services;
using Server.Application.Validators;
using Server.Application.Exceptions;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserValidator _userValidator;
        private readonly IUserRepository _userRepositories;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepositories"></param>
        /// <param name="environment"></param>
        /// <param name="logger"></param>
        /// <param name="userValidator"></param>
        public UserService(
            IUserRepository userRepositories,
            IWebHostEnvironment environment,
            ILogger<UserService> logger,
            UserValidator userValidator)
        {
            _userRepositories = userRepositories;
            _environment = environment;
            _logger = logger;
            _userValidator = userValidator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(UserEntity user)
        {
            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
                throw new ValidationException("Неверные данные при обновлении");

            await _userRepositories.UpdateUserAsync(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatarPath"></param>
        /// <returns></returns>
        public async Task UpdateUserAvatarAsync(Guid userId, string avatarPath)
        {
            var user = await _userRepositories
                .GetUserByIdAsync(userId);

            if (user == null)
                throw new UserException("Пользователь не найден");

            var validationResult = await _userValidator
                .ValidateAsync(user);

            if (!validationResult.IsValid)
                throw new ValidationException(
                    "Неверные данные при обновлении изображения");

            if (!string.IsNullOrEmpty(user.AvatarPath) &&
                !user.AvatarPath.Equals("/avatars/users/default.jpg"))
                {
                    var oldAvatarPath = Path.Combine(_environment.WebRootPath,
                        user.AvatarPath.TrimStart('/'));

                    if (File.Exists(oldAvatarPath))
                        File.Delete(oldAvatarPath);
                }

            user.AvatarPath = avatarPath;

            await _userRepositories.UpdateUserAsync(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepositories.GetUserByIdAsync(id);

            if (user == null)
                throw new UserException("Пользователь не найден");

            var validationResult = await _userValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
                throw new ValidationException("Неверные данные при удалении");

            await _userRepositories.DeleteUserAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            return await _userRepositories.GetUserByUserNameAsync(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByIdAsync(Guid id)
        {
            return await _userRepositories.GetUserByIdAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _userRepositories.GetUserByPhoneNumberAsync(phoneNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        public async Task UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime)
        {
            var user = await _userRepositories.GetUserByIdAsync(userId);

            if (user == null) throw new UserException("Пользователь не найден");

            var validationResult = await _userValidator.ValidateAsync(user);
            if (validationResult.IsValid)
                throw new ValidationException("Ошибка при обновлении токена");

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;

            await _userRepositories.UpdateUserAsync(user);
        }
    }
}
