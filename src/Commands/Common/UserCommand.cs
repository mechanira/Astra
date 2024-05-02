using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Common
{
    [Command("user")]
    public sealed class UserCommand
    {
        private readonly IMongoDatabase Database;

        public UserCommand(DatabaseEngine databaseEngine) => this.Database = databaseEngine.Database;

        [Command("info")]
        public async ValueTask InfoAsync(CommandContext ctx, DiscordUser? user = null)
        {
            await ctx.DeferResponseAsync();

            user ??= ctx.User;

            var userData = await UserModel.FindUserAsync(Database, user.Id);
            long planetsDiscovered = await PlanetModel.CountUserPlanetsAsync(Database, user.Id);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = user.Username + "'s Info",
                Description = $"Credits: ${userData.Money:N0}\nPlanets Discovered: {planetsDiscovered}"
            };

            await ctx.RespondAsync(embedBuilder);
        }

        [Command("update")]
        public async ValueTask UpdateAsync(CommandContext ctx, DiscordUser? user = null)
        {
            await ctx.DeferResponseAsync();

            user ??= ctx.User;

            UserModel userData = new()
            {
                Id = user.Id,
                Username = user.Username,
            };

            await userData.AddAsync(Database);

            await ctx.RespondAsync($"User updated.");
        }
    }
}
