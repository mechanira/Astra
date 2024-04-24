using Astra.Configuration;
using MongoDB.Driver;

namespace Astra.Database
{
    public sealed class DatabaseEngine
    {
        private string ConnectionString { get; init; }
        public IMongoDatabase? Database { get; private set; }

        public DatabaseEngine(AstraConfiguration astraConfiguration)
        {
            DatabaseConfiguration databaseConfiguration = astraConfiguration.Database;
            string host = databaseConfiguration.Host;
            int port = databaseConfiguration.Port;

            ConnectionString = $"mongodb://{host}:{port}";
        }
        
        public void ConnectAsync(string databaseName)
        {
            MongoClient client = new(ConnectionString);

            Database = client.GetDatabase(databaseName);
        }
    }
}
