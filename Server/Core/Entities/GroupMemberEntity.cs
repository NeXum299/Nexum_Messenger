using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Core.Entities
{
    /// <summary>Представляет члена группы.</summary>
    public class GroupMemberEntity
    {
        /// <summary>Идентификатор члена группы.</summary>
        [Key] public Guid Id { get; set; }

        /// <summary>Навигационное свойство, группа.</summary>
        public GroupEntity? Group { get; set; }

        /// <summary>Идентификатор группы, к которой пользователь присоединён.</summary>
        public Guid GroupId { get; set; }

        /// <summary>Идентификатор пользователя.</summary>
        public Guid UserId { get; set; }

        /// <summary>Навигационное свойство, пользователь.</summary>
        public UserEntity? User { get; set; }

        /// <summary>Время присоединения к группе.</summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>Роль пользователя в группе.</summary>
        public string RoleInGroup { get; set; } = "Member";
    }
}
