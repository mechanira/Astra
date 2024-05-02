using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Game
{
    public sealed class ExploreCommand
    {
        private readonly IMongoDatabase Database;

        public ExploreCommand(DatabaseEngine databaseEngine) => this.Database = databaseEngine.Database;

        [Command("explore")]
        public async ValueTask ExecuteAsync(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            int distance = new Random().Next(4, 1000);

            PlanetModel planet = new()
            {
                Distance = distance,
                DiscoveryTime = DateTime.Now,
                DiscoveredBy = ctx.User.Id,
            };

            await planet.CreateNameAsync(Database);

            double mass = planet.Mass;
            string massReferencePlanet = mass > 100 ? "Jupiter" : "Earth"; // if mass exceeds 100 earth masses, use jupiter masses (318x earth)
            double referencePlanetMass = mass > 100 ? mass / 318 : mass;

            await planet.AddAsync(Database);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Planet Discovered",
                Description = $"Name: {planet.Name}\nType: {planet.Type}\nDistance: {distance:N0} LY\nMass: ~{Math.Round(referencePlanetMass, 2)}x {massReferencePlanet}"
            };

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
