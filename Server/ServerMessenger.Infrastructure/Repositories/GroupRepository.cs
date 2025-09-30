using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Interface.Repositories;
using ServerMessenger.Application.Results;
using ServerMessenger.Application.Roles;
using ServerMessenger.Core.Entities;
using ServerMessenger.Infrastructure.Database;

namespace ServerMessenger.Infrastructure.Repositories
{
    /// <summary>Репозиторий для работы с группой.</summary>
    public class GroupRepository : IGroupRepository
    {
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GroupRepository> _logger;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="dbContext">Конекст базы данных.</param>
        /// <param name="logger">Логгирование.</param>
        /// <param name="groupMemberRepository">Репозиторий участников группы.</param>
        public GroupRepository(DatabaseContext dbContext, ILogger<GroupRepository> logger, IGroupMemberRepository groupMemberRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _groupMemberRepository = groupMemberRepository;
        }

        /// <summary>Создаёт новую группу с указанным названием и описанием.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="creator">Создатель группы.</param>
        /// <returns>При успехе, возвращает успешный результат и созданную группу.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с null и описанием ошибки.</returns>
        async public Task<Result<Group>> CreateGroupAsync(Group group, User creator)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.Groups.Add(group);
                await _dbContext.SaveChangesAsync();

                var addMemberResult = await _groupMemberRepository.AddMemberAsync(group, creator, GroupRoles.Owner);
                if (addMemberResult.Fail)
                {
                    await transaction.RollbackAsync();
                    return Result.Error<Group>(null!, addMemberResult.Errors.First());
                }

                try
                {
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating group after commit transaction.");
                    return Result.Error<Group>(null!, "Error creating group.");
                }

                return Result.Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group.");
                return Result.Error<Group>(null!, "Error creating group.");
            }
        }

        /// <summary>Получает группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>При успехе, возвращает успешный результат с найденной группой.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с null и описанием ошибки.</returns>
        async public Task<Result<Group>> GetGroupByIdAsync(Guid groupId)
        {
            try
            {
                var group = await _dbContext.Groups.FirstAsync(g => g.Id == groupId);
                return group == null ? Result.Error<Group>(null!, $"Group not found")
                    : Result.Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get the group {groupId}");
                return Result.Error<Group>(null!, "Error get the group.");
            }
        }

        /// <summary>Получает список всех групп, в которых есть участник.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Возвращает все найденные группы.</returns>
        public async Task<Result<List<Group>>> GetAllGroupByUserId(Guid userId)
        {
            try
            {
                var groups = await _dbContext.Users
                    .Where(gm => gm.Id == userId)
                    .SelectMany(u => u.GroupMemberships)
                    .Include(gm => gm.Group)
                    .Select(gm => gm.Group)
                    .ToListAsync();
                
                return Result.Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get list groups by user id {userId}");
                return Result.Error(new List<Group>(), "Error get list groups by user id.");
            }
        }

        /// <summary>Обновляет данные об группе.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <returns>Обновлённую группу.</returns>
        public async Task<Result<Group>> UpdateGroupAsync(GroupDto groupDto)
        {
            try
            {
                var group = await _dbContext.Groups
                    .Include(g => g.CreatedBy)
                    .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                    .FirstOrDefaultAsync(g => g.Id == groupDto.Id);

                if (group == null)
                    return Result.Error<Group>(null!, "Group not found");

                group.Name = groupDto.Name;
                group.Description = groupDto.Description;
                group.AvatarPath = groupDto.AvatarPath;

                _dbContext.Groups.Update(group);
                await _dbContext.SaveChangesAsync();

                var updatedGroup = await _dbContext.Groups
                    .Include(g => g.CreatedBy)
                    .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                    .FirstAsync(g => g.Id == groupDto.Id);

                return Result.Ok(updatedGroup);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                return Result.Error<Group>(null!, $"Database update error.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error<Group>(null!, $"Unexpected error.");
            }
        }

        /// <summary>Удаляет группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> DeleteGroupAsync(Guid groupId)
        {
            try
            {
                var group = await _dbContext.Groups.FirstAsync(g => g.Id == groupId);

                if (group == null)
                    return Result.Error("Group not found");

                _dbContext.Remove(group);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database update error: {ex.Message}");
                return Result.Error($"Database update error.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return Result.Error($"Unexpected error.");
            }
        }
    }
}
