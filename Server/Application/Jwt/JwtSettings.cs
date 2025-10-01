namespace Server.Application.Jwt
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; }
    }
}
