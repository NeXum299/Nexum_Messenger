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
        public async Task<MessageGroupEntity> SendMessageAsync(Guid groupId, Guid senderId, string content, string? attachmentUrl = null)
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
                return Result.Error<MessageGroup>(null!, "Incorret message");

            var resultSend = await _groupMessageRepository.AddMessageAsync(groupId, senderId, content);
            return resultSend.Fail ? Result.Error<MessageGroup>(null!, "Error send your message")
                : Result.Ok(resultSend.Value); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<List<MessageGroupEntity>> GetMessagesAsync(Guid groupId, Guid userId, int limit = 50)
        {
            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error(new List<MessageGroup>(), "Error get group");

            var isMember = await IsMemberAsync(groupId, userId);
            if (!isMember)
                return Result.Error(new List<MessageGroup>(), "You are not a member of this group");

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
        public async Task<MessageGroupEntity> UpdateMessagesAsync(Guid groupId, Guid userId, TimeUuid messageId, string newContent)
        {
            var messageResult = await _groupMessageRepository.GetMessagesAsync(groupId, 1);
            if (messageResult.Fail || messageResult.Value.First(m => m.MessageId == messageId) == null)
                return Result.Error<MessageGroup>(null!, "Message not found");

            if (messageResult.Value.First(m => m.MessageId == messageId).SenderId != userId)
                return Result.Error<MessageGroup>(null!, "You can only edit your own messages");

            return await _groupMessageRepository.UpdateMessageAsync(groupId, messageId, newContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task DeleteMessageAsync(Guid groupId, Guid userId, TimeUuid messageId)
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
