using Astra.Database.Models;
using Astra.src.Database;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astra.Commands.Game
{
    [Command("search")]
    public sealed class SearchCommand(DatabaseEngine databaseEngine)
    {
        private IMongoDatabase Database = databaseEngine.Database;

        [Command("planet")]
        public async ValueTask ExecuteAsync(CommandContext ctx, string query)
        {
            var collection = Database.GetCollection<PlanetModel>("planets");

            var planet = await collection.Find(x => x.Name.Equals(query, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

            if (planet == null)
            {
                await ctx.RespondAsync("No planet has been found.");
                return;
            }

            string title = "Planet Info • " + planet.Name;
            ulong userId = planet.DiscoveredBy;
            double distance = Math.Round(planet.Distance, 2);
            double mass = Math.Round(planet.Mass, 2);

            DiscordUser discoveryUser = await ctx.Client.GetUserAsync(userId);
            string discoveryUserString = ctx.Guild != null && ctx.Guild.Members.ContainsKey(userId) ? $"<@{userId}>" : discoveryUser.Username;

            string description = $"Discovered by: {discoveryUserString} \n" +
                                 $"Distance: {distance:N0} LY \n" + 
                                 $"Mass: {mass}x Earth \n";

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = title,
                Description = description
            };

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
