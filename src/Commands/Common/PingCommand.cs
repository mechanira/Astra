using DSharpPlus.Commands;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astra.Commands.Common
{
    public static class PingCommand
    {
        [Command("ping"), Description("Returns the bot latency")]
        public static ValueTask ExecuteAsync(CommandContext ctx) => ctx.RespondAsync($":ping_pong: Pong! Latency: `{ctx.Client.Ping}ms`");

        [Command("test"), Description("Not for you")]
        public async static ValueTask TestAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Testing... :black_circle:");

            for (int i = 0; i < 100; i++)
            {
                await ctx.EditResponseAsync("Testing... :red_circle:");
                await ctx.EditResponseAsync("Testing... :black_circle:");
            }
        }
    }
}
