using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Logging;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Core.Entities;

namespace ICassandraSessionProvider.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMessageRepository : IGroupMessageRepository
    {
        private readonly Mapper _mapper;
        private readonly ISession _session;
        private readonly ILogger<GroupMessageRepository> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionProvider"></param>
        /// <param name="logger"></param>
        public GroupMessageRepository(
            Server.Core.Interfaces.ICassandraSessionProvider sessionProvider,
            ILogger<GroupMessageRepository> logger)
        {
            _session = sessionProvider.GetSession();
            _mapper = new Mapper(sessionProvider.GetSession());
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="senderId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task<MessageGroupEntity> AddMessageAsync(
            Guid groupId,
            Guid senderId,
            string content)
        {
            try
            {
                var message = new MessageGroupEntity
                {
                    GroupId = groupId,
                    SenderId = senderId,
                    Content = content,
                    SentAt = DateTimeOffset.UtcNow
                };

                await _mapper.InsertAsync(message, insertNulls: false);

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add message to group {groupId}");
                throw new GroupMessageException("Не удалось добавить сообщение");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="GroupMemberException"></exception>
        public async Task<List<MessageGroupEntity>> GetMessagesAsync(Guid groupId, int limit = 50)
        {
            if (limit <= 0 || limit > 50)
                throw new GroupMemberException("Лимит должен составлять от 1 до 50");
            try
            {
                var messages = (await _mapper.FetchAsync<MessageGroupEntity>
                    ("WHERE group_id = ? ORDER BY message_time_id DESC LIMIT ?", groupId, limit)).ToList();
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages for group {GroupId}", groupId);
                throw new GroupMessageException("Не удалось получить сообщения");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task<MessageGroupEntity> UpdateMessageAsync(Guid groupId, TimeUuid messageId, string newContent)
        {
            try
            {
                var existingMessage = await _mapper.FirstOrDefaultAsync<MessageGroupEntity>
                (
                    "WHERE group_id = ? AND message_time_id = ?", groupId, messageId
                );

                if (existingMessage == null)
                {
                    _logger.LogWarning($"Message {messageId} not found in group {groupId}", messageId, groupId);
                    throw new GroupMessageException("Сообщение не найдено");
                }

                await _mapper.UpdateAsync<MessageGroupEntity>
                (
                    "SET content = ?, is_edited = true WHERE group_id = ? AND message_time_id = ?",
                    newContent, groupId, messageId
                );

                existingMessage.Content = newContent;
                existingMessage.IsEdited = true;

                return existingMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError
                (
                    ex, $"Failed to update message {messageId} in group {groupId}",
                    messageId, groupId
                );
                throw new GroupMessageException("Failed to update message.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task DeleteMessageAsync(Guid groupId, TimeUuid messageId)
        {
            try
            {
                await _mapper.DeleteAsync<MessageGroupEntity>
                (
                    "WHERE group_id = ? AND message_time_id = ?",
                    groupId, messageId
                );

                var exists = await _mapper.FirstOrDefaultAsync<MessageGroupEntity>
                (
                    "WHERE group_id = ? AND message_time_id = ?",
                    groupId, messageId
                );

                if (exists != null)
                {
                    _logger.LogWarning("Delete failed - message {MessageId} not found in group {GroupId}", 
                    messageId, groupId);
                    throw new GroupMessageException("Сообщение не найдено или уже удалено");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError
                (
                    ex, $"Failed to delete message {messageId} from group {groupId}", messageId, groupId
                );
                throw new GroupMessageException("Не удалось удалить сообщение");
            }
        }
    }
}
