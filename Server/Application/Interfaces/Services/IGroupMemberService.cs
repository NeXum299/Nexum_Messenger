using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;

namespace Server.Application.Interface.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupMemberService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <param name="requestingUserId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task AddMemberAsync(Guid groupId, string userName, Guid requestingUserId, string role);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        Task<List<GroupMemberDto>> GetGroupMembersAsync(Guid groupId, Guid requestingUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<int> GetMembersCountAsync(Guid groupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        Task PromoteToAdminAsync(Guid groupId, Guid userId, Guid requestingUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        Task DemoteToMemberAsync(Guid groupId, Guid userId, Guid requestingUserId);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        Task RemoveMemberAsync(Guid groupId, string userName, Guid requestingUserId);
    }
}
