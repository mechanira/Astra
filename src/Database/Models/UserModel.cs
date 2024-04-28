using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Astra.Database.Models
{
    public sealed record UserModel
    {
        [BsonId]
        public required ulong Id { get; set; }
        public string Username { get; set; }
        public ulong Money { get; set; }
        public List<ObjectId> DiscoveredPlanets { get; set; }
        public List<ObjectId> ColonizedPlanets { get; set; }
    }
}
