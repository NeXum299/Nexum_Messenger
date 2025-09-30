using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Logging;
using ServerMessenger.Application.Interface.Repositories;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;
using ServerMessenger.Core.Interfaces;

namespace ServerMessenger.Infrastructure.Repositories
{
    /// <summary>Репозиторий для работы с сообщениями.</summary>
    public class GroupMessageRepository : IGroupMessageRepository
    {
        private readonly Mapper _mapper;
        private readonly ISession _session;
        private readonly ILogger<GroupMessageRepository> _logger;

        /// <summary>Конструктор для инициализации Mapper, ISession и Logger</summary>
        /// <param name="sessionProvider">Сессия ноды.</param>
        /// <param name="logger">Логирование ошибок.</param>
        public GroupMessageRepository(ICassandraSessionProvider sessionProvider, ILogger<GroupMessageRepository> logger)
        {
            _session = sessionProvider.GetSession();
            _mapper = new Mapper(sessionProvider.GetSession());
            _logger = logger;
        }

        /// <summary>Асинхронно создаёт новое сообщение в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="senderId">Идентификатор отправителя.</param>
        /// <param name="content">Содержимое сообщения.</param>
        /// <returns>При успехе, вернёт успешный результат с сообщением.</returns>
        /// <returns>При ошибке, вернёт проваленный результат с null и описанием ошибки.</returns>
        public async Task<Result<MessageGroup>> AddMessageAsync(Guid groupId, Guid senderId, string content)
        {
            try
            {
                var message = new MessageGroup
                {
                    GroupId = groupId,
                    SenderId = senderId,
                    Content = content,
                    SentAt = DateTimeOffset.UtcNow
                };

                await _mapper.InsertAsync(message, insertNulls: false);

                return Result.Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add message to group {groupId}");
                return Result.Error<MessageGroup>(null!, "Failed to add message.");
            }
        }

        /// <summary>Асинхронно получает сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="limit">Лимит получения сообщений за раз.</param>
        /// <returns>При успехе, вернёт успешный результат со списком сообщений.</returns>
        /// <returns>При ошибке, вернёт проваленный результат с пустым списком и описанием ошибки.</returns>
        public async Task<Result<List<MessageGroup>>> GetMessagesAsync(Guid groupId, int limit = 50)
        {
            if (limit <= 0 || limit > 50)
                return Result.Error
                (
                    new List<MessageGroup>(),
                    "Limit must be between 1 and 50"
                );

            try
            {
                var messages = (await _mapper.FetchAsync<MessageGroup>
                    ("WHERE group_id = ? ORDER BY message_time_id DESC LIMIT ?", groupId, limit)).ToList();
                return Result.Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages for group {GroupId}", groupId);
                return Result.Error(
                    value: new List<MessageGroup>(),
                    error: "Failed to get messages."
                );
            }
        }

        /// <summary>Асинхронное обновление сообщения.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения.</param>
        /// <param name="newContent">Новое сообщение.</param>
        /// /// <returns>При успехе, вернёт успешный результат с новым сообщением.</returns>
        /// <returns>При ошибке, вернёт проваленный результат с null и описанием ошибки.</returns>
        public async Task<Result<MessageGroup>> UpdateMessageAsync(Guid groupId, TimeUuid messageId, string newContent)
        {
            try
            {
                var existingMessage = await _mapper.FirstOrDefaultAsync<MessageGroup>
                (
                    "WHERE group_id = ? AND message_time_id = ?", groupId, messageId
                );

                if (existingMessage == null)
                {
                    _logger.LogWarning($"Message {messageId} not found in group {groupId}", messageId, groupId);
                    return Result.Error<MessageGroup>(null!, "Message not found");
                }

                await _mapper.UpdateAsync<MessageGroup>
                (
                    "SET content = ?, is_edited = true WHERE group_id = ? AND message_time_id = ?",
                    newContent, groupId, messageId
                );

                existingMessage.Content = newContent;
                existingMessage.IsEdited = true;

                return Result.Ok(existingMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError
                (
                    ex, $"Failed to update message {messageId} in group {groupId}",
                    messageId, groupId
                );
                return Result.Error<MessageGroup>(null!, "Failed to update message.");
            }
        }

        /// <summary>УдаляеАсинхронно удаляет сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        public async Task<Result> DeleteMessageAsync(Guid groupId, TimeUuid messageId)
        {
            try
            {
                await _mapper.DeleteAsync<MessageGroup>
                (
                    "WHERE group_id = ? AND message_time_id = ?",
                    groupId, messageId
                );

                var exists = await _mapper.FirstOrDefaultAsync<MessageGroup>
                (
                    "WHERE group_id = ? AND message_time_id = ?",
                    groupId, messageId
                );

                if (exists != null)
                {
                    _logger.LogWarning("Delete failed - message {MessageId} not found in group {GroupId}", 
                    messageId, groupId);
                    return Result.Error("Message not found or already deleted.");
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError
                (
                    ex, $"Failed to delete message {messageId} from group {groupId}", messageId, groupId
                );
                return Result.Error($"Failed to delete message.");
            }
        }
    }
}
