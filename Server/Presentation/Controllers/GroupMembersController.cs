using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Interface.Services;
using Server.Application.Roles;

namespace Server.Presentation.Controllers
{
    /// <summary>Контроллер для работы с участниками одной группы.</summary>
    [ApiController]
    [Route("api/group_members")]
    [Authorize]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="groupMemberService">Сервис участников группы.</param>
        public GroupMembersController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        /// <summary>Добавляет участника в группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="userName">Никнейм добавляемого участника.</param>
        /// <response code="200">Участник успешно добавлен.</response>
        /// <response code="400">Некорректные данные.</response>
        /// <response code="403">Нет прав для добавления участника.</response>
        /// <response code="409">Пользователь уже в группе.</response>
        [HttpPost("{groupId}/members/{userName}")]
        public async Task<IActionResult> AddMember([FromRoute] Guid groupId, [FromRoute] string userName)
        {
            var requestingUserId = GetCurrentUserId();
            var result = await _groupMemberService.AddMemberAsync(groupId, userName, requestingUserId, GroupRoles.Member);

            if (result.Fail)
                return result.Errors.First() switch
                {
                    "User is already a member of this group" => Conflict(result.Errors),
                    "User not found" or "Group not found" => NotFound(result.Errors),
                    _ => BadRequest(result.Errors)
                };

            return Ok();
        }

        /// <summary>Получает список участников группы.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <response code="200">Возвращает список участников.</response>
        /// <response code="403">Нет прав для просмотра участников.</response>
        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            var requestingUserId = GetCurrentUserId();

            var result = await _groupMemberService.GetGroupMembersAsync(groupId, requestingUserId);
            
            if (result.Fail) return Forbid();

            return Ok(result.Value);
        }

        /// <summary>Получает количество участников в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <response code="200">Возвращает количество участников.</response>
        /// <response code="404">Группа не найдена.</response>
        [HttpGet("{groupId}/members/count")]
        public async Task<IActionResult> GetMembersCount(Guid groupId)
        {
            var countResult = await _groupMemberService.GetMembersCountAsync(groupId);
            if (countResult.Fail)
                return BadRequest();
                
            return Ok(new { count = countResult.Value });
        }

        /// <summary>Назначает участника администратором.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <response code="200">Роль успешно изменена.</response>
        /// <response code="400">Некорректные данные.</response>
        /// <response code="403">Нет прав для изменения роли.</response>
        [HttpPost("{groupId}/members/{userId}/promote")]
        public async Task<IActionResult> PromoteToAdmin(Guid groupId, Guid userId)
        {
            var requestingUserId = GetCurrentUserId();
            var result = await _groupMemberService.PromoteToAdminAsync(groupId, userId, requestingUserId);
            
            if (result.Fail)
            {     
                return result.Errors.First() switch
                {
                    "User does not have sufficient privileges" => Forbid(),
                    "User is not member of this group" => NotFound(result.Errors),
                    _ => BadRequest(result.Errors)
                };
            }

            return Ok();
        }

        /// <summary>Понижает администратора до участника.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <response code="200">Роль успешно изменена.</response>
        /// <response code="400">Некорректные данные.</response>
        /// <response code="403">Нет прав для изменения роли.</response>
        [HttpPost("{groupId}/members/{userId}/demote")]
        public async Task<IActionResult> DemoteToMember(Guid groupId, Guid userId)
        {
            var requestingUserId = GetCurrentUserId();
            var result = await _groupMemberService.DemoteToMemberAsync(groupId, userId, requestingUserId);
            
            if (result.Fail)
            {       
                return result.Errors.First() switch
                {
                    "Cannot demote the owner of the group" => BadRequest(result.Errors),
                    "User does not have sufficient privileges" => Forbid(),
                    "User is not member of this group" => NotFound(result.Errors),
                    _ => BadRequest(result.Errors)
                };
            }

            return Ok();
        }

        /// <summary>Удаляет участника из группы.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <response code="200">Участник успешно удален.</response>
        /// <response code="400">Некорректные данные.</response>
        /// <response code="403">Нет прав для удаления участника.</response>
        [HttpDelete("{groupId}/members/{userName}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, [FromRoute] string userName)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _groupMemberService.RemoveMemberAsync(groupId, userName, currentUserId);
            
            if (result.Fail)
            {
                return result.Errors.First() switch
                {
                    "Cannot remove the owner of the group" => BadRequest(result.Errors),
                    "User does not have sufficient privileges" => StatusCode(403, result.Errors),
                    "User is not a member of this group" => NotFound(result.Errors),
                    "Member not found" => NotFound(result.Errors),
                    "Group not found" => NotFound(result.Errors),
                    "User not found" => NotFound(result.Errors),
                    _ => BadRequest(result.Errors)
                };
            }

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
