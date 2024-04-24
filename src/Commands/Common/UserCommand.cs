using Astra.Database.Models;
using Astra.src.Database;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astra.Commands.Common
{
    [Command("user")]
    public sealed class UserCommand(DatabaseEngine databaseEngine)
    {
        private IMongoDatabase Database = databaseEngine.Database;

        [Command("info")]
        public async ValueTask InfoAsync(CommandContext ctx, DiscordUser user = null)
        {
            user ??= ctx.User;

            var collection = Database.GetCollection<PlanetModel>("planets");
            var filter = Builders<PlanetModel>.Filter.Eq(x => x.DiscoveredBy, user.Id);

            long planetsDiscovered = await collection.CountDocumentsAsync(filter);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = user.Username + "'s Info",
                Description = $"**Planets Discovered: {planetsDiscovered}**"
            };

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
