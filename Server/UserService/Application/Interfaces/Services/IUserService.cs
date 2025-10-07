using UserSerivce.Application.DTO.Created;
using UserSerivce.Application.DTO.Received;
using UserSerivce.Application.DTO.Updated;

namespace UserSerivce.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserCreatedResponse?> CreateAsync(UserCreatedRequest user, string password);
    Task<UserReceivedResponse?> GetAsync(Guid userId);
    Task<UserUpdatedResponse?> UpdateAsync(UserUpdatedRequest user);
    Task DeleteAsync(Guid userId);
}
