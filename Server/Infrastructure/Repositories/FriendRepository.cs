using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Core.Entities;
using Server.Infrastructure.Database;

namespace Server.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class FriendRepository : IFriendRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<FriendRepository> _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        public FriendRepository(DatabaseContext dbContext, ILogger<FriendRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<FriendEntity?> GetFriendRelationshipAsync(Guid userId, Guid friendId)
        {
            try
            {
                return await _dbContext.Friends
                    .FirstOrDefaultAsync(f => 
                        (f.UserId == userId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friend relationship");
                throw new FriendException("Ошибка при добавлении в друзья");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task AddFriendAsync(Guid userId, Guid friendId)
        {
            try
            {
                var existingRelationship = await GetFriendRelationshipAsync(userId, friendId);
                if (existingRelationship != null)
                    throw new FriendException("Данный друг уже есть у вас в друзьях");

                var friend = new FriendEntity
                {
                    UserId = userId,
                    FriendId = friendId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Friends.Add(friend);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding friend");
                throw new FriendException("Ошибка при добавлении в друзья");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {
            try
            {
                var relationship = await _dbContext.Friends
                    .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == "Pending");
                    
                if (relationship == null)
                    throw new FriendException("Запрос в друзья не найден");

                relationship.Status = "Accepted";
                _dbContext.Friends.Update(relationship);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting friend request");
                throw new FriendException("Ошибка при приеме запроса в друзья");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task RemoveFriendAsync(Guid userId, Guid friendId)
        {
            try
            {
                var relationship = await GetFriendRelationshipAsync(userId, friendId);
                if (relationship == null)
                    throw new FriendException("Ошибка получения друга");

                _dbContext.Friends.Remove(relationship);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing friend");
                throw new FriendException("Ошибка при удалении друга");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FriendDto>> GetFriendsAsync(Guid userId)
        {
            try
            {
                var friends = await _dbContext.Friends
                    .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "Accepted")
                    .Select(f => new FriendDto
                    (
                        f.UserId == userId ? f.FriendId : f.UserId,
                        f.UserId == userId ? f.FriendUser!.UserName : f.User!.UserName,
                        f.UserId == userId ? f.FriendUser!.FirstName : f.User!.FirstName,
                        f.UserId == userId ? f.FriendUser!.LastName : f.User!.LastName,
                        f.UserId == userId ? f.FriendUser!.AvatarPath : f.User!.AvatarPath
                    ))
                    .Distinct()
                    .ToListAsync();

                return friends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friends");
                throw new FriendException("Ошибка при получении друзей");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<FriendRequestDto>> GetIncomingRequestsAsync(Guid userId)
        {
            try
            {
                var incomingRequests = await _dbContext.Friends
                    .Include(f => f.User)
                    .Where(f => f.FriendId == userId && f.Status == "Pending")
                    .Select(f => new FriendRequestDto
                    (
                        f.Id,
                        f.UserId,
                        f.User!.UserName,
                        f.User.FirstName,
                        f.User.LastName,
                        f.User.AvatarPath,
                        f.Status,
                        f.CreatedAt
                    ))
                    .ToListAsync();

                return incomingRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incoming requests");
                throw new FriendException("Ошибка при получении входящих запросов");
            }
        }
    }
}
