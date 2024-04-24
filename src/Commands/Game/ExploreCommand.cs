using Astra.Astra;
using Astra.Configuration;
using Astra.Database.Models;
using Astra.src.Database;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Astra.Commands.Game
{
    public sealed class ExploreCommand(DatabaseEngine databaseEngine)
    {
        private IMongoDatabase Database = databaseEngine.Database;

        [Command("explore")]
        public async ValueTask ExecuteAsync(CommandContext ctx)
        {
            Random rng = new();

            int distance = rng.Next(4, 5_000);

            PlanetModel planet = new()
            {
                Distance = distance,
                DiscoveryTime = DateTime.Now,
                DiscoveredBy = ctx.User.Id,
            };

            await planet.CreateName(Database);

            double mass = planet.Mass;
            string massReferencePlanet = mass > 100 ? "Jupiter" : "Earth";
            double referencePlanetMass = mass > 100 ? mass / 318 : mass;

            await planet.Add(Database);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Planet Discovered",
                Description = $"Name: {planet.Name}\nType: {planet.Type}\nDistance: {distance:N0} LY\nMass: ~{referencePlanetMass:N0}x {massReferencePlanet}"
            };

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
