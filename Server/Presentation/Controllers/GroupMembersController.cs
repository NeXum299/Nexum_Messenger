using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Interface.Services;
using Server.Application.Roles;

namespace Server.Presentation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/group_members")]
    [Authorize]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupMemberService"></param>
        public GroupMembersController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("{groupId}/members/{userName}")]
        public async Task<IActionResult> AddMember([FromRoute] Guid groupId, [FromRoute] string userName)
        {
            var requestingUserId = GetCurrentUserId();
            await _groupMemberService.AddMemberAsync(
                groupId,
                userName,
                requestingUserId,
                GroupRoles.Member.ToString());

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            var requestingUserId = GetCurrentUserId();

            var group = await _groupMemberService.GetGroupMembersAsync(groupId, requestingUserId);

            return Ok(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}/members/count")]
        public async Task<IActionResult> GetMembersCount(Guid groupId)
        {
            var count = await _groupMemberService.GetMembersCountAsync(groupId);

            return Ok(new { count = count });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{groupId}/members/{userId}/promote")]
        public async Task<IActionResult> PromoteToAdmin(Guid groupId, Guid userId)
        {
            var requestingUserId = GetCurrentUserId();
            await _groupMemberService.PromoteToAdminAsync(groupId, userId, requestingUserId);

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{groupId}/members/{userId}/demote")]
        public async Task<IActionResult> DemoteToMember(Guid groupId, Guid userId)
        {
            var requestingUserId = GetCurrentUserId();
            await _groupMemberService.DemoteToMemberAsync(groupId, userId, requestingUserId);

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpDelete("{groupId}/members/{userName}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, [FromRoute] string userName)
        {
            var currentUserId = GetCurrentUserId();
            await _groupMemberService.RemoveMemberAsync(groupId, userName, currentUserId);

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
