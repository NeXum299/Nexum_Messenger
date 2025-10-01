using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;

namespace Server.Application.Interface.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        Task<GroupDto> CreateGroupAsync(GroupDto groupDto, Guid creatorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<GroupDto> GetGroupByIdAsync(Guid groupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<GroupDto>> GetAllGroupByGroupMemberId(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        Task<GroupDto> UpdateGroupAsync(GroupDto groupDto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task RemoveGroupAsync(Guid groupId);
    }
}
