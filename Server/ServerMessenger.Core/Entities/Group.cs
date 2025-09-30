using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServerMessenger.Core.Entities
{
    /// <summary>Представляет группу.</summary>
    public class Group
    {
        /// <summary>Идентификатор группы.</summary>
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Имя группы.</summary>
        public string Name { get; set; } = null!;

        /// <summary>Описание группы.</summary>
        public string Description { get; set; } = "";

        /// <summary>Путь аватарки группы.</summary>
        public string AvatarPath { get; set; } = "/avatars/groups/default.jpg";

        /// <summary>Время создания группы.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Создатель группы.</summary>
        public required User CreatedBy { get; set; }

        /// <summary>Внешний ключ к создателю группы.</summary>
        public Guid CreatedById { get; set; }

        /// <summary>Участники группы.</summary>
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    }
}
