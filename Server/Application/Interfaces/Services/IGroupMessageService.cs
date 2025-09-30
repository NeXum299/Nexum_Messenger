using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Server.Application.Results;
using Server.Core.Entities;

namespace Server.Application.Interface.Services
{
    /// <summary>Сервис для работы с сообщениями.</summary>
    public interface IGroupMessageService
    {
        /// <summary>Отправляет сообщение в указанную группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="senderId">Иденитфикатор отправителя.</param>
        /// <param name="content">Контент сообщения.</param>
        /// <param name="attachmentUrl">Ссылка на какое то вложение.</param>
        /// <returns>Сообщение.</returns>
        Task<Result<MessageGroup>> SendMessageAsync(Guid groupId, Guid senderId, string content, string? attachmentUrl = null);

        /// <summary>Получает историю сообщений.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="limit">Лимит прогрузки сообщений за раз.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список сообщений.</returns>
        Task<Result<List<MessageGroup>>> GetMessagesAsync(Guid groupId, Guid userId, int limit = 50);

        /// <summary>Редактирует сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Иденитфикатор сообщения.</param>
        /// <param name="newContent">Новый контент сообщения.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Отредактированное сообщение.</returns>
        Task<Result<MessageGroup>> UpdateMessagesAsync(Guid groupId, Guid userId, TimeUuid messageId, string newContent);
        
        /// <summary>Удаляет сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Иденитфикатор сообщения.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> DeleteMessageAsync(Guid groupId, Guid userId, TimeUuid messageId);
    }
}
