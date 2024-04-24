using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astra.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class DiscordEventAttribute : Attribute
    {
        public DiscordIntents Intents { get; init; }
        public DiscordEventAttribute(DiscordIntents intents = DiscordIntents.None) => Intents = intents;
    }
}
