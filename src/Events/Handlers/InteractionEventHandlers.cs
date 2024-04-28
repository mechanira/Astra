using Astra.Database;
using Astra.Database.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MongoDB.Driver;

namespace Astra.Events.Handlers
{
    public sealed class InteractionEventHandlers(DatabaseEngine databaseEngine)
    {
        private IMongoDatabase Database = databaseEngine.Database;

        [DiscordEvent]
        public async Task OnComponentInteractionAsync(DiscordClient _, ComponentInteractionCreateEventArgs eventArgs)
        {
            DiscordMessage message = eventArgs.Message;

            if (message.Interaction != null && eventArgs.User != message.Interaction.User)
            {
                await SendMessageAsync(eventArgs.Interaction, "You cannot do the interaction.", true);
                return; // other users won't be able to interact with the components
            }

            switch(eventArgs.Id)
            {
                case "colonyLevelUp":
                    string planetName = eventArgs.Message.Embeds[0].Author.Name;

                    var planet = await PlanetModel.FindAsync(Database, planetName);
                    var colony = planet.Colony;

                    colony.Level++;
                    await planet.AddAsync(Database);

                    await SendMessageAsync(eventArgs.Interaction, $"Colony has been upgraded to level {colony.Level}!", true);

                    DiscordEmbedBuilder embedBuilder = new(message.Embeds[0]);
                    embedBuilder.Fields[0].Value = colony.Level.ToString();

                    await eventArgs.Message.ModifyAsync(embedBuilder.Build()); break;
            }
        }

        private async ValueTask SendMessageAsync(DiscordInteraction ctx, string content, bool ephemeral = false)
        {
            await ctx.DeferAsync(ephemeral);

            DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
                .WithContent(content);

            await ctx.EditOriginalResponseAsync(builder);
        }
    }
}
