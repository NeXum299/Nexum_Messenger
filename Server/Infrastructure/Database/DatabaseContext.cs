using Microsoft.EntityFrameworkCore;
using Server.Core.Entities;

namespace Serverr.Infrastructure.Database
{
    /// <summary>Контекст базы данных для работы с пользователями.</summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>Набор данных пользователей.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Набор данных групп.</summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>Набор данных членов группы.</summary>
        public DbSet<GroupMember> GroupMembers { get; set; }

        /// <summary>Набор данных друзей.</summary>
        public DbSet<Friend> Friends { get; set; }

        /// <summary>Инициализирует новый экземпляр <see cref="DatabaseContext"/>.</summary>
        /// <param name="options">Параметры конфигурации контекста.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>Настраивает модели базы данных.</summary>
        /// <param name="builder">Построитель модель.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(20);

                entity.Property(u => u.LastName).IsRequired().HasMaxLength(25);

                entity.Property(u => u.UserName).IsRequired().HasMaxLength(70);

                entity.Property(u => u.PhoneNumber).IsRequired();

                entity.Property(u => u.AvatarPath).IsRequired();

                entity.Property(u => u.BirthDate);

                entity.Property(u => u.PasswordHash).IsRequired();

                entity.Property(u => u.Role).IsRequired();

                entity.Property(u => u.RefreshToken).IsRequired();

                entity.Property(u => u.RefreshTokenExpiryTime).IsRequired();

                entity.Property(u => u.CreatedAt).IsRequired(); 
            });

            builder.Entity<Group>(entity => {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(g => g.Description)
                    .HasMaxLength(200);

                entity.Property(g => g.AvatarPath)
                    .IsRequired()
                    .HasDefaultValue("/avatars/groups/default.jpg");

                entity.Property(g => g.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(g => g.CreatedBy)
                    .WithMany()
                    .HasForeignKey(g => g.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(gm => gm.Id);
        
                entity.HasOne(gm => gm.User)
                    .WithMany(u => u.GroupMemberships)
                    .HasForeignKey(gm => gm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(gm => gm.Group)
                    .WithMany(g => g.GroupMembers)
                    .HasForeignKey(gm => gm.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.Property(gm => gm.RoleInGroup).IsRequired();
                entity.Property(gm => gm.JoinedAt).IsRequired();
            });

            builder.Entity<Friend>(entity =>
            {
                entity.HasKey(f => f.Id);
                
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Friends)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(f => f.FriendUser)
                    .WithMany()
                    .HasForeignKey(f => f.FriendId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.Property(f => f.CreatedAt).IsRequired();
                entity.Property(f => f.Status).IsRequired().HasMaxLength(20);
                
                entity.HasIndex(f => new { f.UserId, f.FriendId }).IsUnique();
            });
        }
    }
}
