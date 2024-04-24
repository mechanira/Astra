namespace Astra.Configuration
{
    public sealed record DatabaseConfiguration
    {
        public string Host { get; init; } = "localhost";
        public int Port { get; init; } = 27017;
        public string DatabaseName { get; init; } = "astra";
    }
}
