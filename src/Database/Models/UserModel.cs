using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
