using Microsoft.EntityFrameworkCore;
using UserSerivce.Core.Entities;

namespace UserService.Infrastructure.Context
{
    public class UserDbContext : DbContext
    {
        DbSet<UserEntity> Users { get; set; }

        public UserDbContext(
            DbContextOptions<UserDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
