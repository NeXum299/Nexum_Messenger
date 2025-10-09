using UserService.Core.Interfaces.Services;
using UserService.Application.DTO.Created;
using UserService.Application.DTO.Received;
using UserService.Application.DTO.Updated;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Maps;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserCreatedResponse?> CreateAsync(UserCreatedRequest request, string password)
    {
        var user = UserMaps.MapToUser(request);
        var createdUser = await _userRepository.CreateAsync(user, password);
        return UserMaps.MapToCreatedResponse(createdUser);
    }

    public async Task<UserReceivedResponse?> GetAsync(Guid userId)
    {
        var user = await _userRepository.GetAsync(userId);
        return UserMaps.MapToReceivedResponse(user);
    }

    public async Task<UserReceivedResponse?> GetAsync(string userName)
    {
        var user = await _userRepository.GetAsync(userName);
        return UserMaps.MapToReceivedResponse(user);
    }

    public async Task<UserUpdatedResponse?> UpdateAsync(UserUpdatedRequest request)
    {
        var user = UserMaps.MapToUser(request);
        var updatedUser = await _userRepository.UpdateAsync(user);
        return UserMaps.MapToUpdatedResponse(updatedUser);
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _userRepository.DeleteAsync(userId);
    }
}
