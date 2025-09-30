using System;

namespace ServerMessenger.Core.Entities
{
    /// <summary>
    /// Представляет дружеские отношения между двумя пользователями.
    /// </summary>
    public class Friend
    {
        /// <summary>
        /// Уникальный идентификатор записи о дружеских отношениях.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор пользователя, который отправил запрос на дружбу.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который получил запрос на дружбу.
        /// </summary>
        public Guid FriendId { get; set; }

        /// <summary>
        /// Статус дружеских отношений. Возможные значения: "Pending", "Accepted", "Rejected".
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Дата и время создания записи о дружеских отношениях в UTC.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Навигационное свойство для пользователя, отправившего запрос.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство для пользователя, получившего запрос.
        /// </summary>
        public User FriendUser { get; set; } = null!;
    }
}
