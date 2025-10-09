using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;

namespace UserService.Infrastructure.Context;

public class UserDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public UserDbContext(
        DbContextOptions<UserDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(u => u.LastName)
                .HasMaxLength(30);
            entity.Property(u => u.MiddleName)
                .HasMaxLength(20);
            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(u => u.BirthDate);
            entity.Property(u => u.PasswordHash)
                .IsRequired();
            entity.Property(u => u.CreatedAt)
                .IsRequired();
            entity.Property(u => u.AvatarPath)
                .IsRequired();
            entity.Property(u => u.PhoneNumber)
                .IsRequired();
        });
    }
}
