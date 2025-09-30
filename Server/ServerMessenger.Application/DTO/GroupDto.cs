using System;

namespace ServerMessenger.Application.DTO
{
    /// <summary>DTO модель, которая представляет группу.</summary>
    public class GroupDto
    {
        /// <summary>Идентификатор группы.</summary>
        public Guid Id { get; set; }

        /// <summary>Имя группы.</summary>
        public string Name { get; set; } = null!;

        /// <summary>Описание группы.</summary>
        public string Description { get; set; } = "";

        /// <summary>Путь аватарки группы.</summary>
        public string AvatarPath { get; set; } = "/avatars/groups/default.jpg";
    }
}
