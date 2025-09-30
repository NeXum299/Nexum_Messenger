using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Results;
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
        Task<Result<Friend?>> GetFriendRelationshipAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Создает запрос на добавление в друзья.
        /// </summary>
        Task<Result> AddFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Подтверждает входящий запрос на дружбу.
        /// </summary>
        Task<Result> AcceptFriendRequestAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Удаляет дружеские отношения.
        /// </summary>
        Task<Result> RemoveFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// Получает список всех подтвержденных друзей пользователя.
        /// </summary>
        Task<Result<List<FriendDto>>> GetFriendsAsync(Guid userId);

        Task<Result<List<FriendRequestDto>>> GetIncomingRequestsAsync(Guid userId);
    }
}
