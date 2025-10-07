using UserSerivce.Core.Entities;

namespace UserSerivce.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task CreateAsync(UserEntity user, string password);
    Task<UserEntity?> GetAsync(Guid userId);
    Task UpdateAsync(UserEntity user);
    Task DeleteAsync(Guid userId);
}
