using System;
using System.ComponentModel.DataAnnotations;

namespace ServerMessenger.Core.Entities
{
    /// <summary>Представляет члена группы.</summary>
    public class GroupMember
    {
        /// <summary>Идентификатор члена группы.</summary>
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Навигационное свойство, группа.</summary>
        public Group Group { get; set; } = null!;

        /// <summary>Идентификатор группы, к которой пользователь присоединён.</summary>
        public Guid GroupId { get; set; }

        /// <summary>Идентификатор пользователя.</summary>
        public Guid UserId { get; set; }

        /// <summary>Навигационное свойство, пользователь.</summary>
        public User User { get; set; } = null!;

        /// <summary>Время присоединения к группе.</summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Роль пользователя в группе.</summary>
        public string RoleInGroup { get; set; } = "Member";
    }
}
