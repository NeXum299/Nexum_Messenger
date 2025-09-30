using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Results;
using Server.Application.Validators;
using Server.Core.Entities;

namespace Server.Application.Services
{
    /// <summary>Реализация сервиса аутентификации.</summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepositories;
        private readonly LoginModelValidator _loginModelValidator;
        private readonly RegisterModelValidator _registerModelValidator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        /// <summary>Инициализирует новый экземпляр класса <see cref="AuthService"/>.</summary>
        /// <param name="userRepositories">Репозиторий для работы с пользователями.</param>
        /// <param name="passwordHasher">Сервис хеширования паролей.</param>
        /// <param name="logger">Логгер для записи событий.</param>
        /// <param name="loginModelValidator">Валидатор DTO модели логина.</param>
        /// <param name="registerModelValidator">Валидатор DTO модели регистрации.</param>
        public AuthService(IUserRepository userRepositories, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger,
            LoginModelValidator loginModelValidator, RegisterModelValidator registerModelValidator)
        {
            _userRepositories = userRepositories;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _registerModelValidator = registerModelValidator;
            _loginModelValidator = loginModelValidator;
        }

        /// <summary>Регистрирует нового пользователя асинхронно.</summary>
        /// <param name="model">Модель регистрации, содержащая данные пользователя.</param>
        /// <returns>Результат операции. В случае успеха возвращает успешный результат, 
        /// в случае ошибки - сообщение об ошибке.</returns>
        /// <remarks>
        /// При регистрации устанавливается роль "User", дата создания и стандартный аватар.
        /// В случае дублирования номера телефона возвращается соответствующая ошибка.
        /// </remarks>
        public async Task<Result> RegisterUserAsync(RegisterModel model)
        {
            var validationResult = await _registerModelValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return Result.Error("Incorrect data");

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate,
                Role = "User",
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.UtcNow
            };

            try
            {
                var result = await _userRepositories.CreateUserAsync(newUser, model.Password!);
                if (result.Fail)
                    return Result.Error("User creation error");
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to register user (Phone: {model.PhoneNumber})");
                return Result.Error("Error create user.");
            }
        }

        /// <summary>Выполняет вход пользователя асинхронно.</summary>
        /// <param name="model">Модель входа, содержащая номер телефона и пароль.</param>
        /// <returns>Результат операции. В случае успеха возвращает успешный результат,
        /// в случае ошибки - сообщение об ошибке.</returns>
        /// <remarks>
        /// Проверяет наличие пользователя с указанным номером телефона и сверяет хеш пароля.
        /// </remarks>
        public async Task<Result> LoginUserAsync(LoginModel model)
        {
            var validationResult = await _loginModelValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return Result.Error("Incorrect data");

            var result = await _userRepositories.GetUserByPhoneNumberAsync(model.PhoneNumber);
            if (result.Fail)
                return Result.Error("Login error");

            var user = result.Value;
            if (user == null || user.PasswordHash == null)
                return Result.Error("Invalid phone or password");

            var resultVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password!);

            return resultVerification == PasswordVerificationResult.Success
                ? Result.Ok()
                : Result.Error("Invalid password");
        }
    }
}
