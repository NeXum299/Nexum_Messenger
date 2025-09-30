using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerMessenger.Application.Interface.Services;

namespace ServerMessenger.Presentation.Controllers
{
    /// <summary>Контроллер для работы с сообщениями.</summary>
    [ApiController]
    [Route("api/groups/{groupId}/messages")]
    [Authorize]
    public class GroupMessagesController : ControllerBase
    {
        private readonly IGroupMessageService _groupMessageService;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="groupMessageService">Сервис для работы с сообщениями в группе.</param>
        public GroupMessagesController(IGroupMessageService groupMessageService)
        {
            _groupMessageService = groupMessageService;
        }

        /// <summary>Отправляет сообщение в группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="content">Текст сообщения.</param>
        /// <param name="attachmentUrl">Ссылка на вложение (опционально).</param>
        /// <returns>Отправленное сообщение.</returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromRoute] Guid groupId,
            [FromBody] string content, [FromQuery] string? attachmentUrl = null)
        {
            var userId = GetCurrentUserId();
            var result = await _groupMessageService.SendMessageAsync(groupId, userId, content, attachmentUrl);
            return result.Fail ? BadRequest(result.Errors) : Ok(result.Value);
        }

        /// <summary>Получает сообщения из группы.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="limit">Лимит сообщений (по умолчанию 50).</param>
        /// <returns>Список сообщений.</returns>
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromRoute] Guid groupId,
            [FromQuery] int limit = 50)
        {
            var userId = GetCurrentUserId();
            var result = await _groupMessageService.GetMessagesAsync(groupId, userId, limit);
            return result.Fail ? BadRequest(result.Errors) : Ok(result.Value);
        }

        /// <summary>Редактирует сообщение в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения.</param>
        /// <param name="newContent">Новый текст сообщения.</param>
        /// <returns>Отредактированное сообщение.</returns>
        [HttpPut("{messageId}")]
        public async Task<IActionResult> UpdateMessage([FromRoute] Guid groupId,
            [FromRoute] Guid messageId, [FromBody] string newContent)
        {
            var userId = GetCurrentUserId();
            var result = await _groupMessageService.UpdateMessagesAsync(groupId, messageId, userId, newContent);
            return result.Fail ? BadRequest(result.Errors) : Ok(result.Value);
        }

        /// <summary>Удаляет сообщение из группы.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="messageId">Идентификатор сообщения.</param>
        /// <returns>Результат операции.</returns>
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid groupId,
            [FromRoute] Guid messageId)
        {
            var userId = GetCurrentUserId();
            var result = await _groupMessageService.DeleteMessageAsync(groupId, userId, messageId);
            return result.Fail ? BadRequest(result.Errors) : Ok();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID in claims");
            
            return userId;
        }
    }
}
