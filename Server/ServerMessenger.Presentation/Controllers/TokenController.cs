using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServerMessenger.Application.Interface.Services;
using ServerMessenger.Application.Jwt;

namespace ServerMessenger.Presentation.Controllers
{
    /// <summary>Контроллер для работы с токенами.</summary>
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        /// <summary>Инициализирует новый экземпляр TokenController.</summary>
        /// <param name="tokenService">Сервис работы с токенами</param>
        /// <param name="userService">Сервис работы с пользователями</param>
        /// <param name="jwtSettings">Настройки JWT</param>
        public TokenController(ITokenService tokenService, IUserService userService, JwtSettings jwtSettings)
        {
            _tokenService = tokenService;
            _userService = userService;
            _jwtSettings = jwtSettings;
        }

        /// <summary>Обновляет access и refresh токены.</summary>
        /// <returns>Результат обновления токенов</returns>
        /// <response code="200">Токены успешно обновлены</response>
        /// <response code="400">Ошибка при обновлении токенов</response>
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var accessToken = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                return BadRequest("No tokens in cookies");
            }

            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    return BadRequest("Invalid token");

                var result = await _userService.GetUserByIdAsync(userId);

                var user = result.Value;

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return BadRequest("Invalid client request");

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                await _userService.UpdateUserRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

                SetTokenCookies(newAccessToken, newRefreshToken);

                return Ok(new { success = true });
            }
            catch (SecurityTokenException)
            {
                return BadRequest("Invalid token");
            }
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
    }

}
