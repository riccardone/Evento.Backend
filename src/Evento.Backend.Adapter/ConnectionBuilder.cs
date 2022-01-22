using EventStore.ClientAPI;

namespace Evento.Backend.Adapter
{
    public class ConnectionBuilder : IConnectionBuilder
    {
        private readonly Uri _connectionString;
        private readonly ConnectionSettings _connectionSettings;
        private readonly string _connectionName;

        public ConnectionBuilder(Uri connectionString, ConnectionSettings connectionSettings, string connectionName)
        {
            _connectionString = connectionString;
            _connectionSettings = connectionSettings;
            _connectionName = connectionName;
        }

        public object Build(bool openConnection = true)
        {
            var conn = EventStoreConnection.Create(_connectionSettings, _connectionString, _connectionName);
            if (openConnection)
                conn.ConnectAsync().Wait();

            return conn;
        }
    }
}
