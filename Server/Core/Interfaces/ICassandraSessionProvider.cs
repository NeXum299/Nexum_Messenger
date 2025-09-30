using Cassandra;

namespace Server.Core.Interfaces
{
    /// <summary>
    /// Интерфейс провайдера сессии Cassandra.
    /// Предоставляет методы для получения сессии работы с Cassandra и её освобождения.
    /// </summary>
    public interface ICassandraSessionProvider
    {
        /// <summary>Получает сессию для работы с Cassandra.</summary>
        /// <returns>Активная сессия работы с Cassandra.</returns>
        ISession GetSession();

         /// <summary>Освобождает ресурсы, связанные с сессией.</summary>
        void Dispose();
    }
}
