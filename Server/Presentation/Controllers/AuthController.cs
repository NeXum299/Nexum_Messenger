using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTO;
using Server.Application.Interface.Services;
using Server.Application.Jwt;
using Server.Core.Entities;

namespace Server.Presentation.Controllers
{
    /// <summary>Контроллер для обработки запросов аутентификации и авторизации пользователей.</summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        /// <summary>Инициализирует новый экземпляр AuthController.</summary>
        /// <param name="authService">Сервис аутентификации</param>
        /// <param name="tokenService">Сервис работы с токенами</param>
        /// <param name="userService">Сервис работы с пользователями</param>
        /// <param name="jwtSettings">Настройки JWT</param>
        public AuthController(IAuthService authService, ITokenService tokenService,
            IUserService userService, JwtSettings jwtSettings)
        {
            _authService = authService;
            _tokenService = tokenService;
            _userService = userService;
            _jwtSettings = jwtSettings;
        }

        /// <summary>Регистрирует нового пользователя.</summary>
        /// <param name="model">Модель регистрации пользователя</param>
        /// <returns>Результат операции регистрации</returns>
        /// <response code="200">Успешная регистрация</response>
        /// <response code="400">Ошибка при регистрации</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var firstResult = await _authService.RegisterUserAsync(model);

            if (firstResult.Fail)
                return BadRequest(new { success = false, errors = firstResult.Errors });

            var secondResult = await _userService.GetUserByPhoneNumberAsync(model.PhoneNumber!);

            if (secondResult.Fail)
                return BadRequest(new { success = false, error = secondResult.Errors });

            var user = secondResult.Value;

            if (user == null)
                return BadRequest(new { success = false, errors = new[] { "User not found after registration" } });

            return await GenerateAndSetTokens(user);
        }

        /// <summary>Выполняет вход пользователя в систему.</summary>
        /// <param name="model">Модель входа пользователя</param>
        /// <returns>Результат операции входа</returns>
        /// <response code="200">Успешный вход</response>
        /// <response code="400">Ошибка при входе</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var firstResult = await _userService.GetUserByPhoneNumberAsync(model.PhoneNumber!);

            if (firstResult.Fail)
                return BadRequest(new { success = false, errors = new[] { "Ivalid phone or password" } });

            var user = firstResult.Value;

            if (user == null)
                return BadRequest(new { success = false, errors = new[] { "User not found after registration" } });

            var secondResult = await _authService.LoginUserAsync(model);

            if (secondResult.Fail)
                return BadRequest(new { success = false, errors = secondResult.Errors });

            return await GenerateAndSetTokens(user);
        }

        /// <summary>Проверяет статус аутентификации текущего пользователя.</summary>
        /// <returns>Информация об аутентифицированном пользователе</returns>
        /// <response code="200">Пользователь аутентифицирован</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [Authorize]
        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            

            return Ok(new
            {
                userId,
                isAuthenticated = true
            });
        }

        /// <summary>Выполняет выход пользователя из системы.</summary>
        /// <returns>Результат операции выхода</returns>
        /// <response code="200">Успешный выход</response>
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new { success = true });
        }

        /// <summary>Устанавливает токены в cookies ответа.</summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="refreshToken">Refresh token</param>
        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenCookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshCookieOptions);
        }

        /// <summary>/// Генерирует и устанавливает токены для пользователя.</summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Результат с установленными токенами</returns>
        private async Task<IActionResult> GenerateAndSetTokens(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("AvatarPath", user.AvatarPath ?? "/avatars/users/default.jpg")
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _userService.UpdateUserRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

            SetTokenCookies(accessToken, refreshToken);

            return Ok(new
            {
                success = true,
            });
        }
    }
}
