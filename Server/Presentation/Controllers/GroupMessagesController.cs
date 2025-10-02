using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Interface.Services;

namespace Server.Presentation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/groups/{groupId}/messages")]
    [Authorize]
    public class GroupMessagesController : ControllerBase
    {
        private readonly IGroupMessageService _groupMessageService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupMessageService"></param>
        public GroupMessagesController(IGroupMessageService groupMessageService)
        {
            _groupMessageService = groupMessageService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="content"></param>
        /// <param name="attachmentUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage(
            [FromRoute] Guid groupId,
            [FromBody] string content,
            [FromQuery] string? attachmentUrl = null)
        {
            var userId = GetCurrentUserId();
            var message = await _groupMessageService.SendMessageAsync(groupId, userId, content, attachmentUrl);
            return Ok(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromRoute] Guid groupId,
            [FromQuery] int limit = 50)
        {
            var userId = GetCurrentUserId();
            var messages = await _groupMessageService.GetMessagesAsync(groupId, userId, limit);
            return Ok(messages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        [HttpPut("{messageId}")]
        public async Task<IActionResult> UpdateMessage([FromRoute] Guid groupId,
            [FromRoute] Guid messageId, [FromBody] string newContent)
        {
            var userId = GetCurrentUserId();
            var updatedMessage = await _groupMessageService.UpdateMessagesAsync(groupId, messageId, userId, newContent);
            return Ok(updatedMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid groupId,
            [FromRoute] Guid messageId)
        {
            var userId = GetCurrentUserId();
            await _groupMessageService.DeleteMessageAsync(groupId, userId, messageId);
            return Ok();
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
