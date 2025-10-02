using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupMemberRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task AddMemberAsync(GroupEntity group, UserEntity user, string role);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<GroupMemberEntity> GetGroupMemberByIdAsync(Guid memberId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<List<GroupMemberDto>> GetGroupMembersAsync(GroupEntity group);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<GroupMemberEntity> GetMemberByUserIdAndGroupIdAsync(Guid userId, Guid groupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<int> GetMembersCountAsync(Guid groupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task PromoteToAdminAsync(GroupEntity group, UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task DemoteToMemberAsync(GroupEntity group, UserEntity user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        Task RemoveMemberAsync(GroupMemberEntity member);
    }
}
