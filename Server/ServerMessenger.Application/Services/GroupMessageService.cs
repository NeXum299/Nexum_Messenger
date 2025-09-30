using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using ServerMessenger.Application.Interface.Repositories;
using ServerMessenger.Application.Interface.Services;
using ServerMessenger.Application.Results;
using ServerMessenger.Application.Validators;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Services
{
    /// <summary>Сервис для работы с сообщениями.</summary>
    public class GroupMessageService : IGroupMessageService
    {
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupMessageRepository _groupMessageRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly GroupMessageValidator _groupMessageValidator;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="groupMessageRepository">Репозиторий сообщений.</param>
        /// <param name="groupRepository">Репозиторий группы.</param>
        /// <param name="groupMemberRepository">Репозиторий участника.</param>
        /// <param name="groupMessageValidator">Валидатор сообщений.</param>
        public GroupMessageService(IGroupMessageRepository groupMessageRepository,
            IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository,
            GroupMessageValidator groupMessageValidator)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupMessageRepository = groupMessageRepository;
            _groupRepository = groupRepository;
            _groupMessageValidator = groupMessageValidator;
        }

        /// <summary>Отправляет сообщение в указанную группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="senderId">Иденитфикатор отправителя.</param>
        /// <param name="content">Контент сообщения.</param>
        /// <param name="attachmentUrl">Ссылка на какое то вложение.</param>
        /// <returns>Сообщение.</returns>
        public async Task<Result<MessageGroup>> SendMessageAsync(Guid groupId, Guid senderId, string content, string? attachmentUrl = null)
        {
            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error<MessageGroup>(null!, "Group not found.");

            var isMember = await IsMemberAsync(groupId, senderId);
            if (!isMember)
                return Result.Error<MessageGroup>(null!, "You are not a member of this group");

            var message = new MessageGroup
            {
                GroupId = groupId,
                SenderId = senderId,
                Content = content,
                AttachmentUrl = attachmentUrl!
            };

            var validationMessage = await _groupMessageValidator.ValidateAsync(message);
            if (!validationMessage.IsValid)
                return Result.Error<MessageGroup>(null!, "Incorret message");

            var resultSend = await _groupMessageRepository.AddMessageAsync(groupId, senderId, content);
            return resultSend.Fail ? Result.Error<MessageGroup>(null!, "Error send your message")
                : Result.Ok(resultSend.Value); 
        }

        /// <summary>Получает историю сообщений.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="limit">Лимит прогрузки сообщений за раз.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список сообщений.</returns>
        public async Task<Result<List<MessageGroup>>> GetMessagesAsync(Guid groupId, Guid userId, int limit = 50)
        {
            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error(new List<MessageGroup>(), "Error get group");

            var isMember = await IsMemberAsync(groupId, userId);
            if (!isMember)
                return Result.Error(new List<MessageGroup>(), "You are not a member of this group");

            return await _groupMessageRepository.GetMessagesAsync(groupId, limit);
        }

        /// <summary>Редактирует сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Иденитфикатор сообщения.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="newContent">Новый контент сообщения.</param>
        /// <returns>Отредактированное сообщение.</returns>
        public async Task<Result<MessageGroup>> UpdateMessagesAsync(Guid groupId, Guid userId, TimeUuid messageId, string newContent)
        {
            var messageResult = await _groupMessageRepository.GetMessagesAsync(groupId, 1);
            if (messageResult.Fail || messageResult.Value.First(m => m.MessageId == messageId) == null)
                return Result.Error<MessageGroup>(null!, "Message not found");

            if (messageResult.Value.First(m => m.MessageId == messageId).SenderId != userId)
                return Result.Error<MessageGroup>(null!, "You can only edit your own messages");

            return await _groupMessageRepository.UpdateMessageAsync(groupId, messageId, newContent);
        }

        /// <summary>Удаляет сообщение.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="messageId">Иденитфикатор сообщения.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> DeleteMessageAsync(Guid groupId, Guid userId, TimeUuid messageId)
        {
            var messageResult = await _groupMessageRepository.GetMessagesAsync(groupId, 1);
            if (messageResult.Fail || messageResult.Value.First(m => m.MessageId == messageId) == null)
                return Result.Error("Message not found");

            if (messageResult.Value.First(m => m.MessageId == messageId).SenderId != userId)
                return Result.Error<MessageGroup>(null!, "You can only edit your own messages");

            return await _groupMessageRepository.DeleteMessageAsync(groupId, messageId);
        }

        private async Task<bool> IsMemberAsync(Guid groupId, Guid userId)
        {
            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return false;
            
            var memberResult = await _groupMemberRepository.GetGroupMemberByIdAsync(userId);
            if (memberResult.Fail || memberResult.Value == null)
                return false;
            
            if (groupResult.Value.GroupMembers?.Any(m => m.UserId == userId) == true)
                return true;
            
            return false;
        }
    }
}
