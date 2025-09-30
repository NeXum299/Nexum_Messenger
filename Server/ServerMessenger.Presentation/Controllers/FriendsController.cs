using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Interface.Services;

namespace ServerMessenger.Presentation.Controllers
{
    [ApiController]
    [Route("api/friends")]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendService _friendService;

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

        [HttpPost("add")]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            var userId = GetCurrentUserId();
            
            var result = await _friendService.AddFriendAsync(userId, request.UserName);
            
            if (result.Fail)
                return BadRequest(new { success = false, errors = result.Errors });
                
            return Ok(new { success = true, message = "Friend request sent" });
        }

        [HttpPost("accept/{friendId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid friendId)
        {
            var userId = GetCurrentUserId();
            
            var result = await _friendService.AcceptFriendRequestAsync(userId, friendId);
            
            if (result.Fail)
                return BadRequest(new { success = false, errors = result.Errors });
                
            return Ok(new { success = true, message = "Friend request accepted" });
        }

        [HttpDelete("remove/{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var userId = GetCurrentUserId();
            
            var result = await _friendService.RemoveFriendAsync(userId, friendId);
            
            if (result.Fail)
                return BadRequest(new { success = false, errors = result.Errors });
                
            return Ok(new { success = true, message = "Friend removed" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetCurrentUserId();
            
            var result = await _friendService.GetFriendsAsync(userId);
            
            if (result.Fail)
                return BadRequest(new { success = false, errors = result.Errors });
                
            return Ok(new { success = true, value = result.Value });
        }

        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncomingRequests()
        {
            var userId = GetCurrentUserId();
            
            var result = await _friendService.GetIncomingRequestsAsync(userId);
            
            if (result.Fail)
                return BadRequest(new { success = false, errors = result.Errors });
                
            return Ok(new { success = true, value = result.Value });
        }
    }
}
