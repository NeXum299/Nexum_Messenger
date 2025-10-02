using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Roles;
using Server.Core.Entities;
using Server.Infrastructure.Database;

namespace Server.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupRepository : IGroupRepository
    {
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GroupRepository> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="groupMemberRepository"></param>
        public GroupRepository(DatabaseContext dbContext, ILogger<GroupRepository> logger, IGroupMemberRepository groupMemberRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _groupMemberRepository = groupMemberRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        async public Task<GroupEntity> CreateGroupAsync(GroupEntity group, UserEntity creator)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.Groups.Add(group);
                await _dbContext.SaveChangesAsync();

                // добавить отмену транзакции
                await _groupMemberRepository.AddMemberAsync(group, creator, GroupRoles.Owner.ToString());

                try
                {
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating group after commit transaction.");
                    throw new GroupException("Ошибка создания группы");
                }

                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                throw new GroupException("Ошибка создания группы");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        async public Task<GroupEntity> GetGroupByIdAsync(Guid groupId)
        {
            try
            {
                var group = await _dbContext.Groups.FirstAsync(g => g.Id == groupId);
                return group == null
                    ? throw new GroupException("Группа не найдена")
                    : group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get the group {groupId}");
                throw new GroupException("Ошибка получения группы");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<List<GroupEntity>> GetAllGroupByUserId(Guid userId)
        {
            try
            {
                var groups = await _dbContext.Users
                    .Where(gm => gm.Id == userId)
                    .SelectMany(u => u.GroupMemberships)
                    .Include(gm => gm.Group)
                    .Select(gm => gm.Group)
                    .Where(group => group != null)
                    .ToListAsync();
                
                return groups!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get list groups by user id {userId}");
                throw new GroupException("Ошибка получения списка групп по идентификатору пользователя");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<GroupEntity> UpdateGroupAsync(GroupDto groupDto)
        {
            try
            {
                var group = await _dbContext.Groups
                    .Include(g => g.CreatedBy)
                    .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                    .FirstOrDefaultAsync(g => g.Id == groupDto.id);

                if (group == null)
                    throw new GroupException("Группа не найдена");

                group.Name = groupDto.name;
                group.Description = groupDto.description;
                group.AvatarPath = groupDto.avatarPath;

                _dbContext.Groups.Update(group);
                await _dbContext.SaveChangesAsync();

                var updatedGroup = await _dbContext.Groups
                    .Include(g => g.CreatedBy)
                    .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                    .FirstAsync(g => g.Id == groupDto.id);

                return updatedGroup;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                throw new GroupException("Ошибка обновления группы");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new GroupException("Ошибка обновления группы");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task DeleteGroupAsync(Guid groupId)
        {
            try
            {
                var group = await _dbContext.Groups.FirstAsync(g => g.Id == groupId);

                if (group == null)
                    throw new GroupException("Группа не найдена");

                _dbContext.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                throw new GroupException("Ошибка удаления группы");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new GroupException("Ошибка удаления группы.");
            }
        }
    }
}
