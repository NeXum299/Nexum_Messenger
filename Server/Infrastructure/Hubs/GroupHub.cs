using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Application.Interface.Repositories;

namespace Server.Infrastructure.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupHub : Hub
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMessageRepository _groupMessageRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupRepository"></param>
        /// <param name="groupMessageRepository"></param>
        public GroupHub(IGroupRepository groupRepository, IGroupMessageRepository groupMessageRepository)
        {
            _groupMessageRepository = groupMessageRepository;
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task JoinGroup(Guid groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task LeaveGroup(Guid groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="senderId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task SendMessage(Guid groupId, Guid senderId, string content)
        {
            var message = await _groupMessageRepository.AddMessageAsync(groupId, senderId, content);

            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", new {
                GroupdId = groupId,
                MessageId = message.MessageId,
                SenderId = senderId,
                Content = content,
                SentAt = message.SentAt,
                IsEdited = false
            });
        }
    }
}
