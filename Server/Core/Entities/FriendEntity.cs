using System;

namespace Server.Core.Entities
{
    /// <summary>
    /// Представляет дружеские отношения между двумя пользователями.
    /// </summary>
    public class FriendEntity
    {
        /// <summary>
        /// Уникальный идентификатор записи о дружеских отношениях.
        /// </summary>
        public Guid Id { get; set; }

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
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Навигационное свойство для пользователя, отправившего запрос.
        /// </summary>
        public UserEntity? User { get; set; }

        /// <summary>
        /// Навигационное свойство для пользователя, получившего запрос.
        /// </summary>
        public UserEntity? FriendUser { get; set; }
    }
}
