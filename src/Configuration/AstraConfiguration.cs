namespace Astra.Configuration
{
    public sealed record AstraConfiguration
    {
        public required DiscordConfiguration Discord { get; init; }
        public required DatabaseConfiguration Database { get; init; }
        public LoggerConfiguration Logger { get; init; } = new();
    }
}
