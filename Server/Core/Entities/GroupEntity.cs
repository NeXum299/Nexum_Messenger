using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Core.Entities
{
    /// <summary>Представляет группу.</summary>
    public class GroupEntity
    {
        /// <summary>Идентификатор группы.</summary>
        [Key] public Guid Id { get; set; }

        /// <summary>Имя группы.</summary>
        public string? Name { get; set; }

        /// <summary>Описание группы.</summary>
        public string? Description { get; set; }

        /// <summary>Путь аватарки группы.</summary>
        public string AvatarPath { get; set; } = "/avatars/groups/default.jpg";

        /// <summary>Время создания группы.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Создатель группы.</summary>
        public UserEntity? CreatedBy { get; set; }

        /// <summary>Внешний ключ к создателю группы.</summary>
        public Guid CreatedById { get; set; }

        /// <summary>Участники группы.</summary>
        public ICollection<GroupMemberEntity> GroupMembers { get; set; } = new List<GroupMemberEntity>();
    }
}
