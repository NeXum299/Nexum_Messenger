using UserSerivce.Core.Entities;

namespace UserSerivce.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> CreateAsync(UserEntity user, string password);
    Task<UserEntity?> GetAsync(Guid userId);
    Task<UserEntity?> UpdateAsync(UserEntity user);
    Task DeleteAsync(Guid userId);
}
