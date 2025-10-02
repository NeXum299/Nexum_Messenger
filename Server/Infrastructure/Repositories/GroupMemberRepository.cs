using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<GroupMemberRepository> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        public GroupMemberRepository(
            DatabaseContext dbContext,
            ILogger<GroupMemberRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="GroupMemberException"></exception>
        async public Task AddMemberAsync(
            GroupEntity group,
            UserEntity user,
            string role)
        {
            try
            {
                var existingMember = await _dbContext.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
            
                if (existingMember != null)
                    throw new GroupMemberException(
                        "Пользователь уже является учатником этой группы");
                
                var newMember = new GroupMemberEntity
                {
                    GroupId = group.Id,
                    UserId = user.Id,
                    RoleInGroup = role
                };

                _dbContext.GroupMembers.Add(newMember);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding a member to the group");
                throw new GroupMemberException("Ошибка при добавлении участника в группу");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public async Task<GroupMemberEntity> GetGroupMemberByIdAsync(Guid memberId)
        {
            try
            {
                var groupMember = await _dbContext.GroupMembers
                    .FirstOrDefaultAsync(g => g.Id == memberId);

                return groupMember == null
                    ? throw new GroupMemberException("Участник группы не найден")
                    : groupMember;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get the member {memberId}");
                throw new GroupMemberException("Ошибка получения элемента");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<GroupMemberEntity> GetMemberByUserIdAndGroupIdAsync(
            Guid userId, Guid groupId)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);

                return member == null
                    ? throw new GroupMemberException("Участник группы не найден")
                    : member;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member by user and group ID");
                throw new GroupMemberException("Ошибка при получении элемента");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        async public Task<List<GroupMemberDto>> GetGroupMembersAsync(GroupEntity group)
        {
            try
            {
                var members = await _dbContext.GroupMembers.Where(gm => gm.GroupId == group.Id)
                    .Select(gm => new GroupMemberDto
                    (
                        gm.User!.FirstName,
                        gm.User.LastName,
                        gm.User.UserName,
                        gm.User.PhoneNumber,
                        gm.RoleInGroup,
                        DateTime.UtcNow
                    )).ToListAsync();

                return members;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error get all members in group {group.Id}");
                throw new GroupMemberException("Ошибка получения всех участников");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        async public Task<int> GetMembersCountAsync(Guid groupId)
        {
            try
            {
                var countMembers = await _dbContext.GroupMembers
                    .Where(gm => gm.GroupId == groupId)
                    .CountAsync();

                return countMembers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting members count for group {groupId}");
                throw new GroupMemberException("Ошибка при подсчёте количества участников в группе");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async public Task PromoteToAdminAsync(GroupEntity group, UserEntity user)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
                
                member.RoleInGroup = GroupRoles.Admin.ToString();
                _dbContext.GroupMembers.Update(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing the member role to administrator"
                    + $"member {user.Id} in the group {group.Id}");
                throw new GroupMemberException("Ошибка при изменении роли участника в группе");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async public Task DemoteToMemberAsync(GroupEntity group, UserEntity user)
        {
            try
            {
                var member = await _dbContext.GroupMembers
                    .FirstAsync(gm => gm.GroupId == group.Id && gm.UserId == user.Id);
                
                member.RoleInGroup = GroupRoles.Member.ToString();
                _dbContext.GroupMembers.Update(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing the administrator role to member {user.Id} in the group {group.Id}");
                throw new GroupMemberException("Ошибка при изменении роли участника в группе");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        async public Task RemoveMemberAsync(GroupMemberEntity member)
        {
            try
            {
                _dbContext.GroupMembers.Remove(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {member.Id}");
                throw new GroupMemberException("Ошибка при удалении участника группы");
            }
        }
    }
}
