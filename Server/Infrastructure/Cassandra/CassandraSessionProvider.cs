using Cassandra;
using Microsoft.Extensions.Configuration;
using Server.Core.Interfaces;

namespace Server.Infrastructure.Cassandra
{
    /// <summary>
    /// 
    /// </summary>
    public class CassandraSessionProvider : Core.Interfaces.ICassandraSessionProvider
    {
        private readonly Cluster _cluster;
        private readonly ISession _session;

        /// <summary>Инициализирует новый экземпляр провайдера сессии Cassandra.</summary>
        /// <param name="configuration">Конфигурация приложения, содержащая параметры подключения к Cassandra.</param>
        public CassandraSessionProvider(IConfiguration configuration)
        {
            var contactPoints = configuration["Cassandra:ContactPoints"]?.Split(',');
            var keyspace = configuration["Cassandra:Keyspace"];

            _cluster = Cluster.Builder()
                .AddContactPoints(contactPoints)
                .WithPort(int.Parse(configuration["Cassandra:Port"]!))
                .WithCredentials(
                    configuration["Cassandra:Username"],
                    configuration["Cassandra:Password"])
                .Build();

            _session = _cluster.Connect(keyspace);
        }

        /// <summary>Возвращает текущую сессию работы с Cassandra.</summary>
        /// <returns>Текущая активная сессия Cassandra.</returns>
        public ISession GetSession() => _session;

        /// <summary>
        /// Освобождает все ресурсы, связанные с подключением к Cassandra.
        /// Включает закрытие сессии и соединения с кластером.
        /// </summary>
        public void Dispose()
        {
            _session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
