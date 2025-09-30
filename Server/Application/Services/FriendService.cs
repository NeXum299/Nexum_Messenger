using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Results;

namespace Server.Application.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IUserService _userService;
        private readonly ILogger<FriendService> _logger;

        public FriendService(IFriendRepository friendRepository, IUserService userService, ILogger<FriendService> logger)
        {
            _friendRepository = friendRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result> AddFriendAsync(Guid userId, string friendUserName)
        {
            if (string.IsNullOrEmpty(friendUserName))
                return Result.Error("Username is required");

            var friendResult = await _userService.GetUserByUserNameAsync(friendUserName);
            if (friendResult.Fail || friendResult.Value == null)
                return Result.Error("User not found");

            var friend = friendResult.Value;
            
            if (userId == friend.Id)
                return Result.Error("Cannot add yourself as friend");

            return await _friendRepository.AddFriendAsync(userId, friend.Id);
        }

        public async Task<Result> AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {
            var relationshipResult = await _friendRepository.GetFriendRelationshipAsync(friendId, userId);
            if (relationshipResult.Fail || relationshipResult.Value == null)
                return Result.Error("Friend request not found");

            var relationship = relationshipResult.Value;
            
            if (relationship.Status != "Pending")
                return Result.Error("Request is not pending");

            return await _friendRepository.AcceptFriendRequestAsync(userId, friendId);
        }

        public async Task<Result> RemoveFriendAsync(Guid userId, Guid friendId)
        {
            return await _friendRepository.RemoveFriendAsync(userId, friendId);
        }

        public async Task<Result<List<FriendDto>>> GetFriendsAsync(Guid userId)
        {
            return await _friendRepository.GetFriendsAsync(userId);
        }

        public async Task<Result<List<FriendRequestDto>>> GetIncomingRequestsAsync(Guid userId)
        {
            return await _friendRepository.GetIncomingRequestsAsync(userId);
        }
    }
}
