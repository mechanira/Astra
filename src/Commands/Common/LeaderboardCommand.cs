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

            var collection = Database.GetCollection<UserModel>("users");

            var filter = Builders<UserModel>.Filter.Empty;
            var userData = collection.Find(filter)
                .ToList();
            Dictionary<ulong, long> users = new();

            foreach (var user in userData)
            {
                users[user.Id] = await PlanetModel.CountPlanetsAsync(Database, user.Id);
            }
            var sortedUsers = users.OrderByDescending(pair => pair.Value);

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

        [Command("balance")]
        public async ValueTask BalanceLeaderboard(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var collection = Database.GetCollection<UserModel>("users");

            var filter = Builders<UserModel>.Filter.Empty;
            var userData = collection.Find(filter)
                .ToList()
                .OrderByDescending(x => x.Money)
                .Take(10);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Balance",
                Description = "",
            };

            int i = 1;
            foreach (var user in userData)
            {
                embedBuilder.Description += $"#{i} <@{user.Id}> - { user.Money.Humanize() }\n"; i++;
            }

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
