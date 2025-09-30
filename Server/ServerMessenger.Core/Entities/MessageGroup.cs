using System;
using Cassandra;
using Cassandra.Mapping.Attributes;

namespace ServerMessenger.Core.Entities
{
    /// <summary>Представляет сообщения в группе.</summary>
    public class MessageGroup
    {
        /// <summary>Идентификатор группы.</summary>
        [PartitionKey]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        /// <summary>Идентификатор сообщения с временной меткой.</summary>
        [ClusteringKey(1)]
        [Column("message_time_id")]
        public TimeUuid MessageId { get; set; } = TimeUuid.NewId();

        /// <summary>Идентификатор пользователя, отправившего сообщение.</summary>
        [Column("sender_id")]
        public Guid SenderId { get; set; }

        /// <summary>Контент сообщения.</summary>        
        [Column("content")]
        public string Content { get; set; } = "";

        /// <summary>Время отправки сообщения.</summary>
        [Column("sent_at")]
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>Флаг, указывающий, было ли сообщение отредактировано.</summary>
        [Column("is_edited")]
        public bool IsEdited { get; set; } = false;
    
        /// <summary>Ссылка на вложение (изображение, файл и т.д.).</summary>
        [Column("attachment_url")]
        public string AttachmentUrl { get; set; } = string.Empty;
    }
}
