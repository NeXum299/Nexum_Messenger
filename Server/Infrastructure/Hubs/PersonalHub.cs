using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Server.Infrastructure.Hubs 
{
    public class PersonalHub : Hub
    {
        /*private readonly IPersonalMessageRepository _personalMessageRepository;

        public PersonalHub(IPersonalMessageRepository personalMessageRepository)
        {
            _personalMessageRepository = personalMessageRepository;
        }

        public async Task SendPersonalMessage(Guid receiverId, Guid senderId, string content)
        {
            var result = await _personalMessageRepository.AddMessageAsync(receiverId, senderId, content);

            if (result.Success)
            {
                // Отправляем сообщение конкретному пользователю
                await Clients.User(receiverId.ToString()).SendAsync("ReceivePersonalMessage", result.Value);
                await Clients.Caller.SendAsync("ReceivePersonalMessage", result.Value);
            }
        }

        public async Task JoinPersonalChat(Guid friendId)
        {
            // Логика для личных чатов
            await Groups.AddToGroupAsync(Context.ConnectionId, $"personal_{friendId}");
        }*/
    }
}
