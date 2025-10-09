using UserService.Core.Entities;

namespace UserService.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> CreateAsync(UserEntity user, string password);
    Task<UserEntity?> GetAsync(Guid userId);
    Task<UserEntity?> GetAsync(string userName);
    Task<UserEntity?> UpdateAsync(UserEntity user);
    Task DeleteAsync(Guid userId);
}
