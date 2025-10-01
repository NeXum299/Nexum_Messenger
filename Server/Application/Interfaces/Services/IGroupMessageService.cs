using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Server.Core.Entities;

namespace Server.Application.Interface.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupMessageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="senderId"></param>
        /// <param name="content"></param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        Task<MessageGroupEntity> SendMessageAsync(Guid groupId, Guid senderId, string content, string? attachmentUrl = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<MessageGroupEntity>> GetMessagesAsync(Guid groupId, Guid userId, int limit = 50);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        Task<MessageGroupEntity> UpdateMessagesAsync(Guid groupId, Guid userId, TimeUuid messageId, string newContent);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task DeleteMessageAsync(Guid groupId, Guid userId, TimeUuid messageId);
    }
}
