using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Interface.Repositories;
using Server.Application.Results;
using Server.Application.Roles;
using Server.Core.Entities;
using Server.Infrastructure.Database;

namespace Server.Infrastructure.Repositories
{
    /// <summary>Репозиторий для работы с участниками одной группы.</summary>
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GroupMemberRepository> _logger;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="logger">Логгер.</param>
        public GroupMemberRepository(DatabaseContext dbContext, ILogger<GroupMemberRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>Добавляет пользователя в группу с указанной ролью.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="user">Добавляемый пользователь.</param>
        /// <param name="role">Роль добавляемого пользователя.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        async public Task<Result> AddMemberAsync(Group group, User user, string role = GroupRoles.Member)
        {
            try
            {
                var existingMember = await _dbContext.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
            
                if (existingMember != null)
                    return Result.Error("User is already a member of this group");
                
                var newMember = new GroupMember
                {
                    GroupId = group.Id,
                    UserId = user.Id,
                    RoleInGroup = role
                };

                _dbContext.GroupMembers.Add(newMember);
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding a member to the group.");
                return Result.Error("Error adding a member to the group.");
            }
        }

        /// <summary>Получает определённого участника по идентфификатору.</summary>
        /// <param name="memberId">Идентификатор, получаемого участника.</param>
        /// <returns>Возвращает участника группы.</returns>
        public async Task<Result<GroupMember>> GetGroupMemberByIdAsync(Guid memberId)
        {
            try
            {
                var groupMember = await _dbContext.GroupMembers.FirstOrDefaultAsync(g => g.Id == memberId);
                return groupMember == null ? Result.Error<GroupMember>(null!, "Group member not found")
                    : Result.Ok(groupMember);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get the member {memberId}");
                return Result.Error<GroupMember>(null!, "Error get the member.");
            }
        }

        /// <summary>
        /// Получает определённого участника по идентификатору пользователя и идентификатору группы.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Возвращает участника группы.</returns>
        public async Task<Result<GroupMember>> GetMemberByUserIdAndGroupIdAsync(Guid userId, Guid groupId)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
                    
                if (member == null)
                    return Result.Error<GroupMember>(null!, "Member not found");
                    
                return Result.Ok(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member by user and group ID");
                return Result.Error<GroupMember>(null!, "Error getting member");
            }
        }

        /// <summary>Получает всех участников указанной группы.</summary>
        /// <param name="group">Группа.</param>
        /// <returns>При успехе, возвращает успешный результат со списком пользователей.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с пустым списком и описанием ошибки.</returns>
        async public Task<Result<List<GroupMemberDto>>> GetGroupMembersAsync(Group group)
        {
            try
            {
                var members = await _dbContext.GroupMembers.Where(gm => gm.GroupId == group.Id)
                    .Select(gm => new GroupMemberDto
                    {
                        FirstName = gm.User.FirstName!,
                        LastName = gm.User.LastName!,
                        UserName = gm.User.UserName,
                        PhoneNumber = gm.User.PhoneNumber,
                        RoleInGroup = gm.RoleInGroup
                    }).ToListAsync();

                return Result.Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get all members in group {group.Id}");
                return Result.Error(new List<GroupMemberDto>(), "Error get all members.");
            }
        }

        /// <summary>Получает количество участников в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Количество участников.</returns>
        async public Task<Result<int>> GetMembersCountAsync(Guid groupId)
        {
            try
            {
                var countMembers = await _dbContext.GroupMembers
                    .Where(gm => gm.GroupId == groupId)
                    .CountAsync();

                return Result.Ok(countMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting members count for group {groupId}");
                return Result.Error(0, "Error get count members in group");
            }
        }

        /// <summary>Даёт права администратора члену группы.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="user">П0ользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        async public Task<Result> PromoteToAdminAsync(Group group, User user)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
                
                member.RoleInGroup = GroupRoles.Admin;
                _dbContext.GroupMembers.Update(member);
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing the member role to administrator member {user.Id} in the group {group.Id}.");
                return Result.Error("Error changing the role of a member in the group.");
            }
        }

        /// <summary>Изымает права администратора у администратора.</summary>
        /// <param name="group">Групп.</param>
        /// <param name="user">Пользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        async public Task<Result> DemoteToMemberAsync(Group group, User user)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
                
                member.RoleInGroup = GroupRoles.Member;
                _dbContext.GroupMembers.Update(member);
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing the administrator role to member {user.Id} in the group {group.Id}.");
                return Result.Error("Error changing the role of a member in the group.");
            }
        }

        /// <summary>Удаляет пользователя из группы.</summary>
        /// <param name="member">Удаляемый пользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        async public Task<Result> RemoveMemberAsync(GroupMember member)
        {
            try
            {
                _dbContext.GroupMembers.Remove(member);
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {member.Id}.");
                return Result.Error("Error deleting a group member.");
            }
        }
    }
}
