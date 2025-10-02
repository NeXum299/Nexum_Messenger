using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Services;
using Server.Application.Jwt;
using Server.Core.Entities;

namespace Server.Presentation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="tokenService"></param>
        /// <param name="userService"></param>
        /// <param name="jwtSettings"></param>
        /// <param name="logger"></param>
        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            IUserService userService,
            JwtSettings jwtSettings,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _tokenService = tokenService;
            _userService = userService;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                await _authService.RegisterUserAsync(model);
                var user = await _userService
                    .GetUserByPhoneNumberAsync(model.phoneNumber!);

                if (user == null)
                    return BadRequest(new
                    {
                        success = false,
                        errors = new[] {
                            "Пользователь не найден после регистрации" }
                    });
                            
                return await GenerateAndSetTokens(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
            catch (UserException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { "Регистрация не удалась" }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var user = await _userService
                    .GetUserByPhoneNumberAsync(model.phoneNumber!);

                if (user == null)
                    return BadRequest(new
                    {
                        success = false,
                        errors = new[] { "Неверный телефон или пароль" }
                    });
                
                await _authService.LoginUserAsync(model);
                return await GenerateAndSetTokens(user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
            catch (UserException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { "Ошибка входа" }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new { success = true });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<IActionResult> GenerateAndSetTokens(UserEntity user)
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
