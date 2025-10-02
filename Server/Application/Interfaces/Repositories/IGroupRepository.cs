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
    public interface IGroupRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        Task<GroupEntity> CreateGroupAsync(GroupEntity group, UserEntity creator);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<GroupEntity> GetGroupByIdAsync(Guid groupId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<GroupEntity>> GetAllGroupByUserId(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        Task<GroupEntity> UpdateGroupAsync(GroupDto groupDto);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task DeleteGroupAsync(Guid groupId);
    }
}
