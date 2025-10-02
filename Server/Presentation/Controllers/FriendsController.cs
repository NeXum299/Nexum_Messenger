using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTO;
using Server.Application.Interface.Services;

namespace Server.Presentation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/friends")]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendService _friendService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendService"></param>
        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID in claims");
            
            return userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            var userId = GetCurrentUserId();
            
            await _friendService.AddFriendAsync(userId, request.userName);

            return Ok(new { success = true, message = "Отправлен запрос на добавление в друзья" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [HttpPost("accept/{friendId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid friendId)
        {
            var userId = GetCurrentUserId();
            
            await _friendService.AcceptFriendRequestAsync(userId, friendId);
                
            return Ok(new { success = true, message = "Запрос в друзья принят" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        [HttpDelete("remove/{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var userId = GetCurrentUserId();
            
            await _friendService.RemoveFriendAsync(userId, friendId);
                
            return Ok(new { success = true, message = "Пользователь удалён" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetCurrentUserId();
            
            var friend = await _friendService.GetFriendsAsync(userId);
                
            return Ok(new { success = true, value = friend });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncomingRequests()
        {
            var userId = GetCurrentUserId();
            
            var friend = await _friendService.GetIncomingRequestsAsync(userId);
                
            return Ok(new { success = true, value = friend });
        }
    }
}
