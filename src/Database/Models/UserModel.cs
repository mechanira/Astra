using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Astra.Database.Models
{
    public sealed record UserModel
    {
        [BsonId]
        public required ulong Id { get; set; }
        public string Username { get; set; }
        public long Money { get; set; }
        public List<ObjectId> DiscoveredPlanets { get; set; }
        public List<ObjectId> ColonizedPlanets { get; set; }


        public async Task AddAsync(IMongoDatabase database)
        {
            var collection = database.GetCollection<UserModel>("users");

            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, Id);

            await collection.ReplaceOneAsync(filter, this, new ReplaceOptions { IsUpsert = true });
        }

        public static async Task<UserModel> FindUserAsync(IMongoDatabase database, ulong id)
        {
            var collection = database.GetCollection<UserModel>("users");
            var user = await collection.Find(x => x.Id == id).SingleOrDefaultAsync();

            user ??= new UserModel()
            {
                Id = id,
                Username = ""
            };

            return user;
        }

        public bool Buy(long amount) // handle transaction
        {
            if (amount > this.Money)
            {
                return false;
            }

            this.Money -= amount;

            return true;
        }
    }
}
