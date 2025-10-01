namespace Server.Application.DTO
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="password"></param>
    public record class LoginModel(string? phoneNumber, string password);
}
