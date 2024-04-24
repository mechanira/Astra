using Astra.Configuration;
using Astra.src.Database;
using Astra.src.Events;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Commands.Processors.UserCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Globalization;
using System.Reflection;
using DSharpPlusDiscordConfiguration = DSharpPlus.DiscordConfiguration;
using SerilogLoggerConfiguration = Serilog.LoggerConfiguration;

namespace Astra.Astra
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(serviceProvider =>
            {
                ConfigurationBuilder configurationBuilder = new();
                configurationBuilder.Sources.Clear();
                configurationBuilder.AddJsonFile("config.json", true, true);
#if DEBUG
                configurationBuilder.AddJsonFile("config.debug.json", true, true);
#endif
                configurationBuilder.AddEnvironmentVariables();
                configurationBuilder.AddCommandLine(args);

                IConfiguration configuration = configurationBuilder.Build();
                AstraConfiguration? astraConfiguration = configuration.Get<AstraConfiguration>();
                if (astraConfiguration is null)
                {
                    Console.WriteLine("No configuration found. Please modify the config file, set environment variables or pass command line arguments.\nExiting...");
                    Environment.Exit(1);
                }

                return astraConfiguration;
            });

            serviceCollection.AddSingleton(serviceProvider =>
            {
                EventManager eventManager = new(serviceProvider);
                eventManager.GatherEventHandlers(typeof(Program).Assembly);
                return eventManager;
            });

            serviceCollection.AddSingleton(serviceProvider =>
            {
                AstraConfiguration astraConfiguration = serviceProvider.GetRequiredService<AstraConfiguration>();
                if (astraConfiguration.Discord is null || string.IsNullOrEmpty(astraConfiguration.Discord.Token))
                {
                    Console.WriteLine("Token is not set. Exiting...");
                    Environment.Exit(1);
                }

                EventManager eventManager = serviceProvider.GetRequiredService<EventManager>();
                DiscordShardedClient discordClient = new(new DSharpPlusDiscordConfiguration
                {
                    Token = astraConfiguration.Discord.Token,
                    Intents = eventManager.Intents | TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.MessageContents,
                    LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>()
                });

                return discordClient;
            });

            serviceCollection.AddLogging(logging =>
            {
                IServiceProvider serviceProvider = logging.Services.BuildServiceProvider();
                AstraConfiguration astraConfiguration = serviceProvider.GetRequiredService<AstraConfiguration>();
                SerilogLoggerConfiguration serilogLoggerConfiguration = new();
                serilogLoggerConfiguration.WriteTo.Console(
                    formatProvider: CultureInfo.InvariantCulture,
                    outputTemplate: astraConfiguration.Logger.Format,
                    theme: AnsiConsoleTheme.Code
                );

                serilogLoggerConfiguration.WriteTo.File(
                    formatProvider: CultureInfo.InvariantCulture,
                    path: $"{astraConfiguration.Logger.Path}/{astraConfiguration.Logger.FileName}.log",
                    rollingInterval: astraConfiguration.Logger.RollingInterval,
                    outputTemplate: astraConfiguration.Logger.Format
                );

                if (astraConfiguration.Logger.Overrides.Count > 0)
                {
                    foreach ((string key, LogEventLevel value) in astraConfiguration.Logger.Overrides)
                    {
                        serilogLoggerConfiguration.MinimumLevel.Override(key, value);
                    }
                }

                logging.AddSerilog(serilogLoggerConfiguration.CreateLogger());
            });

            serviceCollection.AddSingleton<DatabaseEngine>();


            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            AstraConfiguration astraConfiguration = serviceProvider.GetRequiredService<AstraConfiguration>();
            DatabaseEngine databaseEngine = serviceProvider.GetRequiredService<DatabaseEngine>();
            DiscordShardedClient discordClient = serviceProvider.GetRequiredService<DiscordShardedClient>();
            EventManager eventManager = serviceProvider.GetRequiredService<EventManager>();
            Assembly currentAssembly = typeof(Program).Assembly;

            try
            {
                databaseEngine.ConnectAsync(astraConfiguration.Database.DatabaseName);
            }
            catch (MongoException error)
            {
                serviceProvider.GetRequiredService<ILogger<Program>>().LogError(error, "Failed to connect to the database");
            }

            IReadOnlyDictionary<int, CommandsExtension> commandsExtensions = await discordClient.UseCommandsAsync(new CommandsConfiguration
            {
                ServiceProvider = serviceProvider,
                DebugGuildId = astraConfiguration.Discord.GuildId,
                UseDefaultCommandErrorHandler = false
            });

            foreach (CommandsExtension commandsExtension in commandsExtensions.Values)
            {
                commandsExtension.AddCommands(currentAssembly);

                List<ICommandProcessor> processors = [];
                foreach (string processor in astraConfiguration.Discord.Processors)
                {
                    if (processor.Equals("text", StringComparison.OrdinalIgnoreCase))
                    {
                        TextCommandProcessor textCommandProcessor = new(new()
                        {
                            PrefixResolver = new DefaultPrefixResolver(astraConfiguration.Discord.Prefix ?? throw new InvalidOperationException("Missing Discord Prefix")).ResolvePrefixAsync
                        });

                        textCommandProcessor.AddConverters(currentAssembly);
                        processors.Add(textCommandProcessor);
                    }
                    else if (processor.Equals("slash", StringComparison.OrdinalIgnoreCase))
                    {
                        SlashCommandProcessor slashCommandProcessor = new();
                        slashCommandProcessor.AddConverters(currentAssembly);
                        processors.Add(slashCommandProcessor);
                    }
                    else if (processor.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        processors.Add(new UserCommandProcessor());
                    }
                    else if (processor.Equals("message", StringComparison.OrdinalIgnoreCase))
                    {
                        processors.Add(new MessageCommandProcessor());
                    }
                }

                await commandsExtension.AddProcessorsAsync(processors);
                eventManager.RegisterEventHandlers(commandsExtension);
            }

            eventManager.RegisterEventHandlers(discordClient);

            await discordClient.StartAsync();

            await Task.Delay(-1);
        }
    }
}
