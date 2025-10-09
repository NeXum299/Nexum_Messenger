using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO.Created;
using UserService.Application.DTO.Updated;
using UserService.Core.Events;
using UserService.Core.Interfaces.Services;
using UserService.Application.Interfaces.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IKafkaProducerService _kafkaProducerSerivce;

    public UsersController(
        IUserService userService,
        IKafkaProducerService kafkaProducerSerivce)
    {
        _userService = userService;
        _kafkaProducerSerivce = kafkaProducerSerivce;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreatedRequest request)
    {
        var response = await _userService.CreateAsync(request, request.Password);

        var userCreatedEvent = new UserCreatedEvent
        {
            Id = response.Id,
            UserName = response.UserName,
            FirstName = response.FirstName,
            LastName = response.LastName,
            MiddleName = response.MiddleName,
            PhoneNumber = response.PhoneNumber,
            AvatarPath = response.AvatarPath,
            BirthDate = response.BirthDate,
            CreatedAt = response.CreatedAt
        };

        await _kafkaProducerSerivce.SendEventAsync("user-events", userCreatedEvent);

        return Ok(new { success = true, data = response });

        /*return CreatedAtAction(
            nameof(GetAsync),
            new { userName = response.UserName },
            new { success = true, data = response }
        );*/
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

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UserUpdatedRequest request)
    {
        var response = await _userService.UpdateAsync(request);

        var userUpdatedEvent = new UserUpdatedEvent
        {
            Id = response.Id,
            UserName = response.UserName,
            FirstName = response.FirstName,
            LastName = response.LastName,
            MiddleName = response.MiddleName,
            PhoneNumber = response.PhoneNumber,
            AvatarPath = response.AvatarPath,
            BirthDate = response.BirthDate,
            CreatedAt = response.CreatedAt
        };

        await _kafkaProducerSerivce.SendEventAsync("user-events", userUpdatedEvent);

        return Ok(new
        {
            success = true,
            data = response
        });
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid userId)
    {
        await _userService.DeleteAsync(userId);

        var userDeletedEvent = new UserDeletedEvent
        {
            Id = userId
        };

        await _kafkaProducerSerivce.SendEventAsync("user-events", userDeletedEvent);

        return Ok(new
        {
            success = true,
        });
    }
}
