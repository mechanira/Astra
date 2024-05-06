using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Common
{
    [Command("leaderboard"), TextAlias("lb", "top")]
    public sealed class LeaderboardCommand
    {
        private readonly IMongoDatabase Database;

        public LeaderboardCommand(DatabaseEngine databaseEngine) => this.Database = databaseEngine.Database;

        [Command("planets_discovered"), TextAlias("planets")]
        public async ValueTask PlanetLeaderboard(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var collection = Database.GetCollection<PlanetModel>("planets");

            var filter = Builders<PlanetModel>.Filter.Empty;
            var planets = collection.Find(filter).ToList();
            Dictionary<ulong, long> userPlanets = new();

            foreach (var planet in planets)
            {
                ulong id = planet.DiscoveredBy;

                if (!userPlanets.ContainsKey(id))
                {
                    userPlanets.Add(planet.DiscoveredBy, 0);
                }

                userPlanets[id]++;
            }
            var sortedUsers = userPlanets.OrderByDescending(pair => pair.Value);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Planets Discovered",
                Description = "",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"{await PlanetModel.CountPlanetsAsync(Database)} planets discovered"
                }
            };

            int i = 1;
            foreach (var user in sortedUsers.Take(10))
            {
                embedBuilder.Description += $"#{i} <@{user.Key}> - {user.Value}\n"; i++;
            }

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
