using Microsoft.EntityFrameworkCore;
using UserSerivce.Core.Entities;
using UserSerivce.Core.Interfaces.Repositories;
using UserService.Infrastructure.Context;

namespace UserSerivce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(UserEntity user, string password)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<UserEntity?> GetAsync(Guid userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateAsync(UserEntity user)
    {
        await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(u => u.FirstName, user.FirstName)
                .SetProperty(u => u.LastName, user.LastName)
                .SetProperty(u => u.MiddleName, user.MiddleName)
                .SetProperty(u => u.UserName, user.UserName)
                .SetProperty(u => u.PhoneNumber, user.PhoneNumber)
                .SetProperty(u => u.BirthDate, user.BirthDate)
                .SetProperty(u => u.AvatarPath, user.AvatarPath)
                .SetProperty(u => u.PasswordHash, user.PasswordHash));
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
    }
}
