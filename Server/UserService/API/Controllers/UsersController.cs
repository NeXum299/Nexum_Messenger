using Microsoft.AspNetCore.Mvc;
using UserSerivce.Application.DTO.Created;
using UserSerivce.Core.Interfaces.Services;

namespace UserSerivce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreatedRequest request)
    {
        var response = await _userService.CreateAsync(request, request.Password);
        return CreatedAtAction(
                nameof(GetAsync),
                new { userId = response.Id },
                new { success = true, data = response }
        );
    }

    [HttpGet("{userName}")]
    public async Task<IActionResult> GetAsync(string userName)
    {
        var response = await _userService.GetAsync(userName);
        return Ok(new
        {
            success = true,
            data = response
        });
    }
}
