using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Server.Application.Interface.Services
{
    /// <summary>Интерфейс сервиса для работы с JWT токенами.</summary>
    public interface ITokenService
    {
        /// <summary>Генерирует access токен на основе claims.</summary>
        /// <param name="claims">Набор claims для включения в токен</param>
        /// <returns>Сгенерированный JWT access токен</returns>
        string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>Генерирует refresh токен.</summary>
        /// <returns>Сгенерированный refresh токен.</returns>
        string GenerateRefreshToken();

        /// <summary>Получает principal из истекшего access токена.</summary>
        /// <param name="token">Access токен.</param>
        /// <returns>ClaimsPrincipal пользователя.</returns>
        /// <exception cref="SecurityTokenException">Выбрасывается при невалидном токене</exception>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
