using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Validators;
using Server.Core.Entities;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMessageService : IGroupMessageService
    {
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupMessageRepository _groupMessageRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly GroupMessageValidator _groupMessageValidator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupMessageRepository"></param>
        /// <param name="groupRepository"></param>
        /// <param name="groupMemberRepository"></param>
        /// <param name="groupMessageValidator"></param>
        public GroupMessageService(
            IGroupMessageRepository groupMessageRepository,
            IGroupRepository groupRepository,
            IGroupMemberRepository groupMemberRepository,
            GroupMessageValidator groupMessageValidator)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupMessageRepository = groupMessageRepository;
            _groupRepository = groupRepository;
            _groupMessageValidator = groupMessageValidator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="senderId"></param>
        /// <param name="content"></param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        /// <exception cref="GroupMemberException"></exception>
        /// <exception cref="ValidationException"></exception>
        public async Task<MessageGroupEntity> SendMessageAsync(
            Guid groupId,
            Guid senderId,
            string content,
            string? attachmentUrl = null)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupMessageException("Группа не найдена");

            var isMember = await IsMemberAsync(groupId, senderId);
            if (!isMember)
                throw new GroupMemberException("Вы не являетесь членом этой группы");

            var message = new MessageGroupEntity
            {
                GroupId = groupId,
                SenderId = senderId,
                Content = content,
                AttachmentUrl = attachmentUrl!
            };

            var validationMessage = await _groupMessageValidator.ValidateAsync(message);
            if (!validationMessage.IsValid)
                throw new ValidationException("Невалидное сообщение");

            return await _groupMessageRepository.AddMessageAsync(groupId, senderId, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task<List<MessageGroupEntity>> GetMessagesAsync(
            Guid groupId,
            Guid userId,
            int limit = 50)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupMessageException("Ошибка получения группы");

            var isMember = await IsMemberAsync(groupId, userId);
            if (!isMember)
                throw new GroupMessageException("Вы не являетесь участником этой группы");

            return await _groupMessageRepository.GetMessagesAsync(groupId, limit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task<MessageGroupEntity> UpdateMessagesAsync(
            Guid groupId,
            Guid userId,
            TimeUuid messageId,
            string newContent)
        {
            var message = await _groupMessageRepository.GetMessagesAsync(groupId, 1);

            if (message.First(m => m.MessageId == messageId).SenderId != userId)
                throw new GroupMessageException("Вы можете редактировать только свои собственные сообщения");

            return await _groupMessageRepository.UpdateMessageAsync(groupId, messageId, newContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        /// <exception cref="GroupMessageException"></exception>
        public async Task DeleteMessageAsync(Guid groupId, Guid userId, TimeUuid messageId)
        {
            var messageResult = await _groupMessageRepository.GetMessagesAsync(groupId, 1);
            if (messageResult.First(m => m.MessageId == messageId) == null)
                throw new GroupMessageException("Сообщение не найдено");

            if (messageResult.First(m => m.MessageId == messageId).SenderId != userId)
                throw new GroupMessageException("Вы можете редактировать только свои собственные сообщения");

            await _groupMessageRepository.DeleteMessageAsync(groupId, messageId);
        }

        private async Task<bool> IsMemberAsync(Guid groupId, Guid userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null) return false;
            
            var member = await _groupMemberRepository.GetGroupMemberByIdAsync(userId);
            if (member == null) return false;
            
            if (group.GroupMembers?.Any(m => m.UserId == userId) == true)
                return true;
            
            return false;
        }
    }
}
