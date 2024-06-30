using DSharpPlus.Commands;

namespace Astra.Commands.Game
{
    public sealed class TutorialCommand
    {
        [Command("tutorial")]
        public async ValueTask ExecuteAsync(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            string tutorial = "- Explore planets with `/explore` \n" +
                           "- Create colonies on planets and earn credits \n" +
                           "- Upgrade your colonies to increase profit";

            await ctx.RespondAsync(tutorial);
        }
    }
}
