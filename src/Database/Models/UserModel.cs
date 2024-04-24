using MongoDB.Bson;

namespace Astra.Database.Models
{
    public sealed record UserModel
    {
        public required ulong Id { get; set; }
        public string Username { get; set; }
        public long Credits { get; set; }
        public List<ObjectId> DiscoveredPlanets { get; set; }
    }
}
