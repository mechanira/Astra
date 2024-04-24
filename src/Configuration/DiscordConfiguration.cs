namespace Astra.Configuration
{
    public sealed record DiscordConfiguration
    {
        public required string? Token { get; init; }
        public string Prefix { get; init; } = "a";
        public ulong GuildId { get; init; }
        public string[] Processors { get; init; } = [];
    }
}
