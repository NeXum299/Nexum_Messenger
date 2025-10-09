using UserService.Application.DTO.Created;
using UserService.Application.DTO.Received;
using UserService.Application.DTO.Updated;

namespace UserService.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserCreatedResponse?> CreateAsync(UserCreatedRequest user, string password);
    Task<UserReceivedResponse?> GetAsync(Guid userId);
    Task<UserReceivedResponse?> GetAsync(string userName);
    Task<UserUpdatedResponse?> UpdateAsync(UserUpdatedRequest user);
    Task DeleteAsync(Guid userId);
}
