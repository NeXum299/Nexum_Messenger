using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTO;
using Server.Application.Interface.Services;

namespace Server.Presentation.Controllers
{
    /// <summary>Контроллер для работы с группами.</summary>
    [ApiController]
    [Route("api/groups")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="groupService">Сервис группы.</param>
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>Создаёт группу.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <response code="200">Успешное создание группы.</response>
        /// <response code="400">Ошибка при создании группы.</response>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDto groupDto)
        {
            var userId = GetCurrentUserId();

            var resultCreate = await _groupService.CreateGroupAsync(groupDto, userId);

            if (resultCreate.Fail || resultCreate.Value == null)
                return BadRequest(new { success = false, errors = resultCreate.Errors});

            return CreatedAtAction(
                nameof(GetGroup),
                new { groupId = resultCreate.Value.Id },
                new { success = true, value = resultCreate.Value }
            );
        }

        /// <summary>Получает информацию о группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <response code="200">Возвращает информацию о группе.</response>
        /// <response code="404">Группа не найдена.</response>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
                return BadRequest(new { success = false, errors = "Invalid group ID"});

            var result = await _groupService.GetGroupByIdAsync(groupId);

            return (result.Fail || result.Value == null)
                ? NotFound(new { success = false, errors = result.Errors})
                : Ok(new { success = true, value = result.Value});
        }

        /// <summary>Возвращает список групп, в которых состоит текущий пользователь.</summary>
        /// <returns>Список групп.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllGroup()
        {
            var userId = GetCurrentUserId();
            var result = await _groupService.GetAllGroupByGroupMemberId(userId);
            return (result.Fail || result.Value == null) ? BadRequest(result.Errors)
                : Ok(new { success = true, value = result.Value});
        }

        /// <summary>Обновляет группу по идентификатору.</summary>
        /// <param name="groupDto">Идентификатор группы.</param>
        /// <returns>Обновленная группа.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupDto groupDto)
        {
            var result = await _groupService.UpdateGroupAsync(groupDto);

            if (result.Fail || result.Value == null)
                return BadRequest();
            
            return Ok(result.Value);
        }

        /// <summary>Удаление группы по идентификатору.</summary>
        /// <param name="groupId"></param>
        /// <returns>Результат операции.</returns>
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid groupId)
        {
            var result = await _groupService.RemoveGroupAsync(groupId);

            if (result.Fail)
                return BadRequest(new { success = false,
                    errors = result.Errors?.FirstOrDefault() ?? "Ошибка при удалении группы"});
            
            return Ok(new { message = "Вы успешно покинули группу", success = true});
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
