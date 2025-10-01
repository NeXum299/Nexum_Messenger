using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IUserService _userService;
        private readonly ILogger<FriendService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendRepository"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        public FriendService(
            IFriendRepository friendRepository,
            IUserService userService,
            ILogger<FriendService> logger)
        {
            _friendRepository = friendRepository;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendUserName"></param>
        /// <returns></returns>
        public async Task AddFriendAsync(Guid userId, string friendUserName)
        {
            if (string.IsNullOrEmpty(friendUserName))
                throw new FriendException("Неверные данные");

            var friend = await _userService
                .GetUserByUserNameAsync(friendUserName);
            if (friend == null)
                throw new FriendException("Пользователь не найден");

            if (userId == friend.Id)
                throw new FriendException("Нельзя добавить себя в друзья");

            await _friendRepository.AddFriendAsync(userId, friend.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        /// <exception cref="FriendException"></exception>
        public async Task AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {
            var relationship = await _friendRepository
                .GetFriendRelationshipAsync(friendId, userId);

            if (relationship == null)
                throw new FriendException("Запрос не найден");

            if (relationship.Status != "Pending")
                throw new FriendException("Запрос не находится на рассмотрении");

            await _friendRepository.AcceptFriendRequestAsync(userId, friendId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task RemoveFriendAsync(Guid userId, Guid friendId)
        {
            await _friendRepository.RemoveFriendAsync(userId, friendId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FriendDto>> GetFriendsAsync(Guid userId)
        {
            return await _friendRepository.GetFriendsAsync(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FriendRequestDto>> GetIncomingRequestsAsync(Guid userId)
        {
            return await _friendRepository.GetIncomingRequestsAsync(userId);
        }
    }
}
