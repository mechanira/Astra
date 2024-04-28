using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Game
{
    [Command("colony")]
    public sealed class ColonyCommand(DatabaseEngine databaseEngine)
    {
        IMongoDatabase Database = databaseEngine.Database;

        [Command("menu"), DefaultGroupCommand]
        public async ValueTask ColonyAsync(CommandContext ctx, string planetName)
        {
            var planet = await PlanetModel.FindAsync(Database, planetName);

            if (planet == null) { await ctx.RespondAsync("Please enter a valid planet name."); return; }
            if (planet.Colony == null) { await ctx.RespondAsync($"Colony on planet `{planet.Name}` already exists."); return; }

            var colony = planet.Colony;

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = $"Colony Menu",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = planet.Name
                }
            };
            embedBuilder.AddField("Level", colony.Level.ToString(), true);
            embedBuilder.AddField("Output", colony.MoneyOutput.ToString(), true);

            if (colony.Owner != ctx.User.Id) { await ctx.RespondAsync(embedBuilder); return; }

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embedBuilder)
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(DiscordButtonStyle.Success, "colonyLevelUp", "Level Up")
                });

            await ctx.RespondAsync(messageBuilder);
        }

        [Command("create")]
        public async ValueTask CreateAsync(CommandContext ctx, string planetName)
        {
            var planet = await PlanetModel.FindAsync(Database, planetName);

            if (planet == null) { await ctx.RespondAsync("No planet found."); return; }
            if (planet.Colony != null) { await ctx.RespondAsync($"Colony on planet `{planet.Name}` already exists."); return; }

            ColonyModel colony = new()
            {
                Owner = ctx.User.Id,
                CreatedAt = DateTime.Now
            };
            planet.Colony = colony;
            await planet.AddAsync(Database);

            await ctx.RespondAsync("Colony has been successfully created!");
        }
    }


}
