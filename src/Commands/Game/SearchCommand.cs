using Astra.Database;
using Astra.Database.Models;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using MongoDB.Driver;

namespace Astra.Commands.Game
{
    [Command("search")]
    public sealed class SearchCommand
    {
        private readonly IMongoDatabase Database;

        public SearchCommand(DatabaseEngine databaseEngine) => this.Database = databaseEngine.Database;

        [Command("planet")]
        public async ValueTask PlanetAsync(CommandContext ctx, string query)
        {
            await ctx.DeferResponseAsync();

            var collection = Database.GetCollection<PlanetModel>("planets");

            var planet = await collection.Find(x => x.Name.Equals(query, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

            if (planet == null)
            {
                await ctx.RespondAsync("No planet has been found.");
                return;
            }

            string type = planet.Type;
            ulong userId = planet.DiscoveredBy;
            double distance = Math.Round(planet.Distance, 2); // round the value by 2 decimal spaces
            double mass = Math.Round(planet.Mass, 2);
            long unix = new DateTimeOffset(planet.DiscoveryTime).ToUnixTimeSeconds();

            DiscordUser discoveryUser = await ctx.Client.GetUserAsync(userId);
            string discoveryUsername = ctx.Guild != null && ctx.Guild.Members.ContainsKey(userId) ? $"<@{userId}>" : discoveryUser.Username;

            string imageUrl = "https://media.discordapp.net/attachments/1229767113384661052/1234151921917366314/opera_8TUpU453U8.png?ex=662fb10b&is=662e5f8b&hm=754d658d1b755159d1e7161acf7c78ee018a5bc27be5c8fa2796ab4376f8890c&=&format=webp&quality=lossless&width=673&height=673";
            string description = $"Type: {type} \n" +
                                 $"Distance: {distance:N0} LY \n" +
                                 $"Mass: {mass}x Earth \n" +
                                 $"Discovered by: {discoveryUsername} (<t:{unix}:R>)\n";

            if (planet.Colony != null)
            {
                description += $"\n**Colony**\n" +
                               $"Level {planet.Colony.Level}\n" +
                               $"Output: ${planet.Colony.MoneyOutput:N0} per hour";
            }

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Planet Info",
                Description = description,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = planet.Name,
                },
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = imageUrl,
                    Width = 128,
                    Height = 128
                }
            };

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
