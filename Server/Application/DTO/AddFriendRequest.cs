namespace Server.Application.DTO
{
    /// <summary>
    /// DTO добавление друга.
    /// </summary>
    /// <param name="userName">Никнейм друга.</param>
    public record class AddFriendRequest(string userName);
}
