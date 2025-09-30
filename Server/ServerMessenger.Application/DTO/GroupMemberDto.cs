using System;

namespace ServerMessenger.Application.DTO
{
    /// <summary>DTO модель, которая представляет участника группы.</summary>
    public class GroupMemberDto
    {
        /// <summary>Имя участника.</summary>
        public string FirstName { get; set; } = null!;

        /// <summary>Фамилия участника.</summary>
        public string LastName { get; set; } = null!;

        /// <summary>Никнейм пользователя.</summary>
        public string UserName { get; set; } = null!;

        /// <summary>Номер телефона участника.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Роль в группе у участника.</summary>
        public string RoleInGroup { get; set; } = null!;

        /// <summary>Время присоединения участника к группе.</summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
