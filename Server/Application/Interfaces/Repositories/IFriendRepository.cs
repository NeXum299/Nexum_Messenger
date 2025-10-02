using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFriendRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        Task<FriendEntity?> GetFriendRelationshipAsync(Guid userId, Guid friendId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        Task AddFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        Task AcceptFriendRequestAsync(Guid userId, Guid friendId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        Task RemoveFriendAsync(Guid userId, Guid friendId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<FriendDto>> GetFriendsAsync(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<FriendRequestDto>> GetIncomingRequestsAsync(Guid userId);
    }
}
