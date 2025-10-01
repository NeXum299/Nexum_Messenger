using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Validators;
using Server.Core.Entities;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepositories;
        private readonly LoginModelValidator _loginModelValidator;
        private readonly RegisterModelValidator _registerModelValidator;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepositories"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="logger"></param>
        /// <param name="loginModelValidator"></param>
        /// <param name="registerModelValidator"></param>
        public AuthService(
            IUserRepository userRepositories,
            IPasswordHasher<UserEntity> passwordHasher,
            ILogger<AuthService> logger,
            LoginModelValidator loginModelValidator,
            RegisterModelValidator registerModelValidator)
        {
            _userRepositories = userRepositories;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _registerModelValidator = registerModelValidator;
            _loginModelValidator = loginModelValidator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task RegisterUserAsync(RegisterModel model)
        {
            var validationResult = await _registerModelValidator
                .ValidateAsync(model);

            if (!validationResult.IsValid)
                throw new ValidationException("Неверные регистрационные данные");

            var newUser = new UserEntity
            {
                FirstName = model.firstName,
                LastName = model.lastName,
                UserName = model.userName,
                PhoneNumber = model.phoneNumber,
                BirthDate = model.birthDate,
                Role = "User",
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.UtcNow
            };

            await _userRepositories.CreateUserAsync(newUser, model.password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task LoginUserAsync(LoginModel model)
        {
            var validationResult = await _loginModelValidator
                .ValidateAsync(model);

            if (!validationResult.IsValid)
                throw new UserException("Неверные данные для входа");

            var user = await _userRepositories.GetUserByPhoneNumberAsync(model.phoneNumber);

            if (user == null)
                throw new UserException(
                    "Неверный номер телефона или пароль");

            if (user.PasswordHash == null)
                throw new UserException(
                    "Неверный номер телефона или пароль");

            var resultVerification = _passwordHasher.
                VerifyHashedPassword(user, user.PasswordHash,
                    model.password);

            if (resultVerification != PasswordVerificationResult.Success)
                throw new UserException("Неверный номер телефона или пароль");
        }
    }
}
