using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using UserService.Application.Interfaces.Repositories;
using UserService.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public UserRepository(
        UserDbContext dbContext,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserEntity?> CreateAsync(UserEntity user, string password)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        user.CreatedAt = DateTime.UtcNow;
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
    }

    public async Task<UserEntity?> GetAsync(Guid userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task<UserEntity?> GetAsync(string userName)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<UserEntity?> UpdateAsync(UserEntity user)
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

        return await GetAsync(user.Id);
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
    }
}
