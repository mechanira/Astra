using System;
using System.Linq;
using System.Threading.Tasks;
using Astra.Events;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.Commands.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Astra.Events.Handlers
{
    public sealed class ErrorHandler
    {
        [DiscordEvent]
        public static async Task OnErroredAsync(CommandsExtension _, CommandErroredEventArgs eventArgs)
        {
            if (eventArgs.Exception is CommandNotFoundException commandNotFoundException)
            {
                await eventArgs.Context.RespondAsync($"Command not found: {commandNotFoundException.CommandName}");
                return;
            }

            if (eventArgs.Exception is CommandNotExecutableException commandNotExecutableException)
            {
                await eventArgs.Context.RespondAsync($"Command failed to execute: {commandNotExecutableException.InnerException}");
            }

            switch (eventArgs.Exception)
            {
                case DiscordException discordError:
                    await eventArgs.Context.RespondAsync($"# Command Error\nHttp Code: {discordError.Response?.StatusCode.ToString() ?? "Not provided."}\nError Message: {discordError.JsonMessage ?? "Not provided"}");
                    break;
            }
        }
    }
}
