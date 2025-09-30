using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Interface.Repositories
{
    /// <summary>Интерфейс репозитория для работы с сообщениями.</summary>
    public interface IGroupMessageRepository
    {
        /// <summary>Добавляет новое сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="senderId">Идентификатор отправителя.</param>
        /// <param name="content">Текст сообщения.</param>
        /// <returns>Добавленное сообщение.</returns>
        Task<Result<MessageGroup>> AddMessageAsync(Guid groupId, Guid senderId, string content);

        /// <summary>Получает сообщения из указанной группы.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="limit">Максимальное количество возвращаемых сообщений (по умолчанию 50).</param>
        /// <returns>Сообщения.</returns>
        Task<Result<List<MessageGroup>>> GetMessagesAsync(Guid groupId, int limit = 50);

        /// <summary>Обновляет текст сообщения.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения.</param>
        /// <param name="newContent">Новый текст сообщения.</param>
        /// <returns>Обновлённое сообщение.</returns>
        Task<Result<MessageGroup>> UpdateMessageAsync(Guid groupId, TimeUuid messageId, string newContent);
        
        /// <summary>Удаляет сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения (TimeUuid).</param>
        Task<Result> DeleteMessageAsync(Guid groupId, TimeUuid messageId);
    }
}
