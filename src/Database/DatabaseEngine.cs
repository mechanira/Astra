using Astra.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astra.src.Database
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
