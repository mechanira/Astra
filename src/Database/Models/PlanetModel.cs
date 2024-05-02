using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Astra.Database.Models
{
    public sealed record PlanetModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Name { get; private set; }
        public string Type { get; init; }
        public double Mass { get; init; }
        public double Distance { get; set; }
        public ulong DiscoveredBy { get; set; }
        public DateTime DiscoveryTime { get; set; }
        public ColonyModel? Colony { get; set; }

        public PlanetModel()
        {
            Random rng = new();

            string[] types = ["Terrestrial", "Super-Earth", "Ice Giant", "Gas Giant"];
            Type = types[rng.Next(types.Length)];

            switch(Type)
            {
                case "Terrestrial": Mass = rng.NextDouble(0.0125, 15); break;

                case "Super-Earth": Mass = rng.NextDouble(2, 10); break;

                case "Ice Giant": Mass = rng.NextDouble(5, 20); break;

                case "Gas Giant": Mass = rng.NextDouble(150, 600); break;
            }
        }

        public async Task AddAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<PlanetModel>("planets");

            var filter = Builders<PlanetModel>.Filter.Eq(x => x.Id, Id);

            await collection.ReplaceOneAsync(filter, this, new ReplaceOptions { IsUpsert = true });
        }


        public async Task CreateNameAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<PlanetModel>("planets");

            var filter = Builders<PlanetModel>.Filter.Empty;
            var projection = Builders<PlanetModel>.Projection.Expression(x => x.Name);

            var documents = await collection.Find(filter).Project(projection).ToListAsync();

            var numbers = documents
            .SelectMany(name => Regex.Matches(name, @"\d+").Cast<Match>().Select(m => int.Parse(m.Value)));

            int highestNumber = numbers.Any() ? numbers.Max() : 0;

            Name = $"AST {highestNumber + 1} b";
        }

        public static async Task<PlanetModel?> FindAsync(IMongoDatabase database, string name)
        {
            var collection = database.GetCollection<PlanetModel>("planets");

            var result = await collection.Find(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

            if (result == null) return null;

            return result;
        }

        public static async Task<long> CountUserPlanetsAsync(IMongoDatabase database, ulong userId)
        {
            var planetCollection = database.GetCollection<PlanetModel>("planets");
            var filter = Builders<PlanetModel>.Filter.Eq(x => x.DiscoveredBy, userId);

            return await planetCollection.CountDocumentsAsync(filter);
        }

        public static async Task<long> CountAllPlanetsAsync(IMongoDatabase database)
        {
            var planetCollection = database.GetCollection<PlanetModel>("planets");
            var filter = Builders<PlanetModel>.Filter.Empty;

            return await planetCollection.CountDocumentsAsync(filter);
        }
    }


    public sealed record ColonyModel
    {
        public ulong Owner { get; set; } 
        public int Level { get; set; } = 1;
        public ulong MoneyOutput { get; set; } = 100;
        public DateTime CreatedAt { get; set; }

        public ulong LevelUpAmount()
        {
            var amount = 1000 * Math.Pow(1.5, Level - 1);

            return (ulong)amount;
        }
    }
}
