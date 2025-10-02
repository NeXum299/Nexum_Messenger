using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupMessageRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="senderId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<MessageGroupEntity> AddMessageAsync(Guid groupId, Guid senderId, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<MessageGroupEntity>> GetMessagesAsync(Guid groupId, int limit = 50);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        Task<MessageGroupEntity> UpdateMessageAsync(Guid groupId, TimeUuid messageId, string newContent);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task DeleteMessageAsync(Guid groupId, TimeUuid messageId);
    }
}
