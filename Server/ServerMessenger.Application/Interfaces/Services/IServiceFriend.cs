using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Interface.Services
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
