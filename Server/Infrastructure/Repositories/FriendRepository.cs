using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Interface.Repositories;
using Server.Application.Results;
using Server.Core.Entities;
using Server.Infrastructure.Database;

namespace Server.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для управления дружескими отношениями между пользователями.
    /// Обеспечивает операции по добавлению, подтверждению, удалению друзей и получению информации о дружеских связях.
    /// </summary>
    public class FriendRepository : IFriendRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<FriendRepository> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FriendRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных для работы с сущностями друзей.</param>
        /// <param name="logger">Логгер для записи информации о выполнении операций и ошибках.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если dbContext или logger равны null.</exception>
        public FriendRepository(DatabaseContext dbContext, ILogger<FriendRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Получает информацию о дружеских отношениях между двумя пользователями.
        /// </summary>
        /// <param name="userId">Идентификатор первого пользователя.</param>
        /// <param name="friendId">Идентификатор второго пользователя.</param>
        /// <returns>
        /// Результат операции, содержащий объект <see cref="Friend"/> с информацией о отношениях,
        /// или null если отношения не найдены. В случае ошибки возвращает результат с ошибкой.
        /// </returns>
        public async Task<Result<Friend?>> GetFriendRelationshipAsync(Guid userId, Guid friendId)
        {
            try
            {
                var relationship = await _dbContext.Friends
                    .FirstOrDefaultAsync(f => 
                        (f.UserId == userId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == userId));

                return Result.Ok(relationship);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friend relationship");
                return Result.Error<Friend?>(null, "Error getting friend relationship");
            }
        }

        /// <summary>
        /// Создает запрос на добавление в друзья от одного пользователя другому.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, отправляющего запрос на дружбу.</param>
        /// <param name="friendId">Идентификатор пользователя, которому отправляется запрос на дружбу.</param>
        /// <returns>
        /// Результат операции. В случае успеха возвращает успешный результат,
        /// в случае ошибки или если отношения уже существуют - результат с ошибкой.
        /// </returns>
        /// <remarks>
        /// Создает запись в таблице Friends со статусом "Pending".
        /// </remarks>
        public async Task<Result> AddFriendAsync(Guid userId, Guid friendId)
        {
            try
            {
                var existingRelationship = await GetFriendRelationshipAsync(userId, friendId);
                if (existingRelationship.Value != null)
                    return Result.Error("Friend relationship already exists");

                var friend = new Friend
                {
                    UserId = userId,
                    FriendId = friendId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Friends.Add(friend);
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding friend");
                return Result.Error("Error adding friend");
            }
        }

        /// <summary>
        /// Подтверждает входящий запрос на дружбу от другого пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, подтверждающего запрос на дружбу.</param>
        /// <param name="friendId">Идентификатор пользователя, от которого был отправлен запрос на дружбу.</param>
        /// <returns>
        /// Результат операции. В случае успеха возвращает успешный результат,
        /// в случае ошибки или если запрос не найден - результат с ошибкой.
        /// </returns>
        /// <remarks>
        /// Изменяет статус существующей записи с "Pending" на "Accepted".
        /// </remarks>
        public async Task<Result> AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {
            try
            {
                var relationship = await _dbContext.Friends
                    .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId && f.Status == "Pending");
                    
                if (relationship == null)
                    return Result.Error("Friend request not found");

                relationship.Status = "Accepted";
                _dbContext.Friends.Update(relationship);
                await _dbContext.SaveChangesAsync();
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting friend request");
                return Result.Error("Error accepting friend request");
            }
        }

        /// <summary>
        /// Удаляет дружеские отношения между двумя пользователями.
        /// </summary>
        /// <param name="userId">Идентификатор первого пользователя.</param>
        /// <param name="friendId">Идентификатор второго пользователя.</param>
        /// <returns>
        /// Результат операции. В случае успеха возвращает успешный результат,
        /// в случае ошибки или если отношения не найдены - результат с ошибкой.
        /// </returns>
        /// <remarks>
        /// Удаляет запись о дружбе независимо от ее текущего статуса.
        /// </remarks>
        public async Task<Result> RemoveFriendAsync(Guid userId, Guid friendId)
        {
            try
            {
                var relationship = await GetFriendRelationshipAsync(userId, friendId);
                if (relationship.Value == null)
                    return Result.Error("Friend relationship not found");

                _dbContext.Friends.Remove(relationship.Value);
                await _dbContext.SaveChangesAsync();
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing friend");
                return Result.Error("Error removing friend");
            }
        }

        /// <summary>
        /// Получает список всех подтвержденных друзей указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, для которого получается список друзей.</param>
        /// <returns>
        /// Результат операции, содержащий список объектов <see cref="User"/> с информацией о друзьях.
        /// В случае ошибки возвращает результат с ошибкой и пустым списком.
        /// </returns>
        /// <remarks>
        /// Возвращает только друзей со статусом "Accepted".
        /// </remarks>
        public async Task<Result<List<FriendDto>>> GetFriendsAsync(Guid userId)
        {
            try
            {
                var friends = await _dbContext.Friends
                    .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "Accepted")
                    .Select(f => new FriendDto
                    {
                        Id = f.UserId == userId ? f.FriendId : f.UserId,
                        UserName = f.UserId == userId ? f.FriendUser.UserName : f.User.UserName,
                        FirstName = f.UserId == userId ? f.FriendUser.FirstName : f.User.FirstName,
                        LastName = f.UserId == userId ? f.FriendUser.LastName : f.User.LastName,
                        AvatarPath = f.UserId == userId ? f.FriendUser.AvatarPath : f.User.AvatarPath
                    })
                    .Distinct()
                    .ToListAsync();
                    
                return Result.Ok(friends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friends");
                return Result.Error<List<FriendDto>>(new List<FriendDto>(), "Error getting friends");
            }
        }

        public async Task<Result<List<FriendRequestDto>>> GetIncomingRequestsAsync(Guid userId)
        {
            try
            {
                var incomingRequests = await _dbContext.Friends
                    .Include(f => f.User)
                    .Where(f => f.FriendId == userId && f.Status == "Pending")
                    .Select(f => new FriendRequestDto
                    {
                        Id = f.Id,
                        UserId = f.UserId,
                        UserName = f.User.UserName,
                        FirstName = f.User.FirstName,
                        LastName = f.User.LastName,
                        AvatarPath = f.User.AvatarPath,
                        Status = f.Status,
                        CreatedAt = f.CreatedAt
                    })
                    .ToListAsync();
                    
                return Result.Ok(incomingRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incoming requests");
                return Result.Error<List<FriendRequestDto>>(new List<FriendRequestDto>(), "Error getting incoming requests");
            }
        }
    }
}
