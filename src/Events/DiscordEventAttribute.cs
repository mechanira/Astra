using DSharpPlus;

namespace Astra.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class DiscordEventAttribute : Attribute
    {
        public DiscordIntents Intents { get; init; }
        public DiscordEventAttribute(DiscordIntents intents = DiscordIntents.None) => Intents = intents;
    }
}
