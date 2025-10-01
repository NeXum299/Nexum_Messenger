using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>
    /// Определяет контракт для репозитория управления дружескими отношениями.
    /// </summary>
    public interface IFriendRepository
    {
        /// <summary>
        /// Получает информацию о дружеских отношениях между двумя пользователями.
        /// </summary>
        Task<FriendEntity?> GetFriendRelationshipAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Создает запрос на добавление в друзья.
        /// </summary>
        Task AddFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Подтверждает входящий запрос на дружбу.
        /// </summary>
        Task AcceptFriendRequestAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Удаляет дружеские отношения.
        /// </summary>
        Task RemoveFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Получает список всех подтвержденных друзей пользователя.
        /// </summary>
        Task<List<FriendDto>> GetFriendsAsync(Guid userId);

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<FriendRequestDto>> GetIncomingRequestsAsync(Guid userId);
    }
}
