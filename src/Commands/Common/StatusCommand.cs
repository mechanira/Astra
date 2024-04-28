using DSharpPlus.Commands;
using DSharpPlus.Entities;
using System.Diagnostics;

namespace Astra.Commands.Common
{
    public sealed class StatusCommand
    {
        [Command("status")]
        public async ValueTask ExecuteAsync(CommandContext ctx)
        {
            var currentProcess = Process.GetCurrentProcess();

            TimeSpan startTime = DateTime.UtcNow - currentProcess.StartTime.ToUniversalTime();
            double memory = (double)currentProcess.PrivateMemorySize64 / 1_000_000;

            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = "Bot Status"
            };

            embedBuilder.AddField("Uptime", TextCommandUtils.TimeSpanToString(startTime));
            embedBuilder.AddField("Memory usage", $"{Math.Round(memory, 2)} MB");

            await ctx.RespondAsync(embedBuilder);
        }
    }
}
