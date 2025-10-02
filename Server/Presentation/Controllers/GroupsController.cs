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
    [Route("api/groups")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupService"></param>
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDto groupDto)
        {
            var userId = GetCurrentUserId();

            var createdGroup = await _groupService.CreateGroupAsync(groupDto, userId);

            if (createdGroup == null)
                return BadRequest(new { success = false });

            return CreatedAtAction(
                nameof(GetGroup),
                new { groupId = createdGroup.id },
                new { success = true, value = createdGroup }
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
                return BadRequest(new { success = false, errors = "Неверный идентификатор группы" });

            var groupDto = await _groupService.GetGroupByIdAsync(groupId);

            return (groupDto == null)
                ? NotFound(new { success = false})
                : Ok(new { success = true, value = groupDto});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllGroup()
        {
            var userId = GetCurrentUserId();
            var groupsDto = await _groupService.GetAllGroupByGroupMemberId(userId);
            return Ok(new { success = true, value = groupsDto });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupDto groupDto)
        {
            var updatedGroupDto = await _groupService.UpdateGroupAsync(groupDto);
            if (updatedGroupDto == null) return BadRequest();
            return Ok(updatedGroupDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid groupId)
        {
            await _groupService.RemoveGroupAsync(groupId);
            
            return Ok(new { success = true, message = "Вы успешно покинули группу" });
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
