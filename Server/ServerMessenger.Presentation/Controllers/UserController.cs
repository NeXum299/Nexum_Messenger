using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerMessenger.Application.Interface.Services;

namespace ServerMessenger.Presentation.Controllers
{
    /// <summary>Контроллер для работы с пользователями.</summary>
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IUserService _userService;

        /// <summary>Инициализирует новый экземпляр UserController.</summary>
        /// <param name="environment">Окружение веб-хоста</param>
        /// <param name="userService">Сервис работы с пользователями</param>
        public UserController(IWebHostEnvironment environment, IUserService userService)
        {
            _environment = environment;
            _userService = userService;
        }

        /// <summary>Загружает аватар пользователя.</summary>
        /// <param name="file">Файл аватара</param>
        /// <returns>Результат загрузки аватара</returns>
        /// <response code="200">Аватар успешно загружен</response>
        /// <response code="400">Ошибка при загрузке аватара</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("update-avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, error = "No file uploaded" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return BadRequest(new { success = false, error = "Invalid file type" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { success = false, error = "File size exceeds 5MB limit" });

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "avatars", "users");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var avatarPath = $"/avatars/users/{fileName}";

            var firstResult = await _userService.UpdateUserAvatarAsync(userId, avatarPath);
            if (firstResult.Fail)
                return BadRequest(new { success = false, error = firstResult.Errors.FirstOrDefault() ?? "Failed to update avatar" });

            var secondResult = await _userService.GetUserByIdAsync(userId);
            if (secondResult.Fail || secondResult.Value == null)
                return BadRequest(new { success = false, error = secondResult.Errors.FirstOrDefault() ?? "Failed to get user data by id" });

            var updatedUser = secondResult.Value;

            return Ok(new { success = true, avatarPath = $"{Request.Scheme}://{Request.Host}{updatedUser.AvatarPath}" });
        }

        /// <summary>Получает имя и фамилию пользователя.</summary>
        /// <returns>Имя и фамилия пользователя</returns>
        /// <response code="200">Успешное получение данных</response>
        /// <response code="400">Ошибка при получении данных</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpGet("name")]
        public async Task<IActionResult> GetName()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _userService.GetUserByIdAsync(userId);

            if (result.Fail)
                return BadRequest(new { success = false, error = result.Errors.FirstOrDefault() ?? "Failed to get user data by id" });
            if (result.Value == null)
                return NotFound(new { success = false, error = result.Errors.FirstOrDefault() ?? "User not found" });

            var user = result.Value;

            return Ok(new
            {
                success = true,
                firstName = user.FirstName,
                lastName = user.LastName
            });
        }

        /// <summary>Получает URL аватара пользователя.</summary>
        /// <returns>URL аватара пользователя</returns>
        /// <response code="200">Успешное получение аватара</response>
        /// <response code="400">Ошибка при получении аватара</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpGet("avatar")]
        public async Task<IActionResult> GetAvatar()
        {
            Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("Expires", "0");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _userService.GetUserByIdAsync(userId);

            if (result.Fail)
                return BadRequest(new { success = false, error = result.Errors.First() ?? "Failed to get user data by id" });
            if (result.Value == null)
                return NotFound(new { success = false, error = result.Errors.First() ?? "User not found" });

            var user = result.Value;

            if (user == null || string.IsNullOrEmpty(user.AvatarPath))
            {
                return Ok(new
                {
                    success = true,
                    avatarPath = $"{Request.Scheme}://{Request.Host}/avatars/default.jpg"
                });
            }

            return Ok(new
            {
                success = true,
                avatarPath = $"{Request.Scheme}://{Request.Host}{user.AvatarPath}"
            });
        }
    }
}
