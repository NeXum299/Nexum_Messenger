using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Server.Application.Interface.Repositories;
using Server.Application.Results;

namespace Server.Infrastructure.Hubs
{
    /// <summary>SignalR хаб для обработки чат-сообщений и управления группами.</summary>
    public class GroupHub : Hub
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMessageRepository _groupMessageRepository;

        /// <summary>Инициализирует новый экземпляр класса <see cref="GroupHub"/>.</summary>
        /// <param name="groupRepository">Репозиторий для работы с группами.</param>
        /// <param name="groupMessageRepository">Репозиторий для работы с сообщениями групп.</param>
        public GroupHub(IGroupRepository groupRepository, IGroupMessageRepository groupMessageRepository)
        {
            _groupMessageRepository = groupMessageRepository;
            _groupRepository = groupRepository;
        }

        /// <summary>Добавляет текущее подключение к указанной группе.</summary>
        /// <param name="groupId">Идентификатор группы для присоединения.</param>
        public async Task JoinGroup(Guid groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        /// <summary>Удаляет текущее подключение из указанной группы.</summary>
        /// <param name="groupId">Идентификатор группы для выхода.</param>
        public async Task LeaveGroup(Guid groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        /// <summary>Отправляет сообщение в указанную группу.</summary>
        /// <param name="groupId">Идентификатор группы, куда отправляется сообщение.</param>
        /// <param name="senderId">Идентификатор отправителя сообщения.</param>
        /// <param name="content">Текст сообщения.</param>
        /// <returns>Результат операции отправки сообщения. В случае успеха возвращает <see cref="Result.Ok"/>,
        /// в случае ошибки - <see cref="Result.Error(string)"/> с сообщением об ошибке.</returns>
        public async Task<Result> SendMessage(Guid groupId, Guid senderId, string content)
        {
            var result = await _groupMessageRepository.AddMessageAsync(groupId, senderId, content);

            if (result.Fail)
                return Result.Error("Couldn't send message");

            var message = result.Value;

            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", new {
                GroupdId = groupId,
                MessageId = message.MessageId,
                SenderId = senderId,
                Content = content,
                SentAt = message.SentAt,
                IsEdited = false
            });

            return Result.Ok();
        }
    }
}
