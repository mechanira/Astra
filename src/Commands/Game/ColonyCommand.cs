using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Game
{
    [Command("colony")]
    public sealed class ColonyCommand
    {
        private readonly IMongoDatabase Database;

        public ColonyCommand(DatabaseEngine databaseEngine) => this.Database = databaseEngine.Database;

        [Command("view"), DefaultGroupCommand]
        public async ValueTask ColonyAsync(CommandContext ctx, string planetName)
        {
            await ctx.DeferResponseAsync();

            var planet = await PlanetModel.FindAsync(Database, planetName);

            if (planet == null) { await ctx.RespondAsync("Please enter a valid planet name."); return; }

            ColonyModel colony = planet.Colony;

            if (colony == null) { await ctx.RespondAsync($"Planet doesn't have a colony."); return; }

            string levelUpAmount = AstraUtilities.Humanize(colony.LevelUpAmount());
            string description = $"Upgrade to level {colony.Level + 1}: ${levelUpAmount}";

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = $"Colony Menu",
                Description = description,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = planet.Name.DisplayPlanetName()
                }
            };
            embedBuilder.AddField("Level", colony.Level.ToString(), true);
            embedBuilder.AddField("Output", $"${ AstraUtilities.Humanize(colony.MoneyOutput) } per hour", true);

            if (colony.Owner != ctx.User.Id) { await ctx.RespondAsync(embedBuilder); return; }

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embedBuilder)
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(DiscordButtonStyle.Success, "colonyLevelUp", "Level Up")
                });

            await ctx.RespondAsync(messageBuilder);
        }

        [Command("user")]
        public async ValueTask UserAsync(CommandContext ctx, DiscordUser? user = null)
        {
            await ctx.DeferResponseAsync();

            user ??= ctx.User;

            var collection = Database.GetCollection<PlanetModel>("planets");
            var filter = Builders<PlanetModel>.Filter.Eq(x => x.Colony.Owner, user.Id);
            var planets = collection.Find(filter).ToList().OrderByDescending(x => x.Colony.Level);

            string description = string.Empty;

            foreach (var planet in planets.Take(10))
            {
                var colony = planet.Colony;

                string humanizedValue = AstraUtilities.Humanize(colony.MoneyOutput);

                description += $"{planet.Name.DisplayPlanetName()} - Level {colony.Level} - ${humanizedValue}/h" + "\n";
            }
            if (planets.Count() > 10) { description += "..."; }

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "User Colonies",
                Description = description == string.Empty ? "*No colonies found*" : description,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = user.Username,
                    IconUrl = user.AvatarUrl
                }
            };

            await ctx.RespondAsync(embedBuilder);
        }

        [Command("create")]
        public async ValueTask CreateAsync(CommandContext ctx, string planetName)
        {
            await ctx.DeferResponseAsync();

            var planet = await PlanetModel.FindAsync(Database, planetName);

            if (planet == null) { await ctx.RespondAsync("No planet found."); return; }
            if (planet.Colony != null) { await ctx.RespondAsync($"Colony on planet `{ planet.Name.DisplayPlanetName() }` already exists."); return; }

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
