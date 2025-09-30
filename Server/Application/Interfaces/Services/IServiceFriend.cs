using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Results;
using Server.Core.Entities;

namespace Server.Application.Interface.Services
{
    public interface IFriendService
    {
        Task<Result> AddFriendAsync(Guid userId, string friendUserName);
        Task<Result> AcceptFriendRequestAsync(Guid userId, Guid friendId);
        Task<Result> RemoveFriendAsync(Guid userId, Guid friendId);
        Task<Result<List<FriendDto>>> GetFriendsAsync(Guid userId);
        Task<Result<List<FriendRequestDto>>> GetIncomingRequestsAsync(Guid userId);
    }
}
