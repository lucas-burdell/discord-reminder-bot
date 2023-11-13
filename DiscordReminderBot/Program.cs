using Discord.WebSocket;
using DiscordReminderBot.Core;
using DiscordReminderBot.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordReminderBot
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true)
                    .AddEnvironmentVariables()
                    .Build();
            var botConfig = new BotConfig();
            config.Bind("DiscordReminderBot", botConfig);

            var serviceProvider = await LoadDependencyInjection(botConfig);


            var server = new Server(botConfig, serviceProvider.GetRequiredService<IDiscordLogger>(), serviceProvider.GetRequiredService<DiscordSocketClient>());
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                cancellationTokenSource.Cancel();
            };

            await server.StartAsync(cancellationTokenSource.Token);
        }

        private static async Task<ServiceProvider> LoadDependencyInjection(BotConfig config)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(config);
            var logger = new LoggerFactory(config).BuildLogger();
            serviceCollection.AddSingleton(logger);
            serviceCollection.AddSingleton((provider) => new ClientFactory(config, provider.GetRequiredService<IDiscordLogger>()).BuildClient());
            Assembly assembly = Assembly.GetExecutingAssembly();
            var listeners = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && (type.Name.EndsWith("Listener") || type.Name.EndsWith("Service") || type.Name.EndsWith("Repo")))
                {
                    if (type.Name.EndsWith("Listener") && type.GetInterface(nameof(IDiscordListener)) != null)
                    {
                        listeners.Add(type);
                        serviceCollection.AddSingleton(type);
                    } else
                    {
                        var interfaceList = type.GetInterfaces();
                        var interfaceType = interfaceList.Where(x => x.Namespace?.EndsWith("Interfaces") ?? false).First();

                        serviceCollection.AddSingleton(interfaceType, type);
                    }
                }
            }

            var provider = serviceCollection.BuildServiceProvider();

            foreach (var listener in listeners)
            {
                ((IDiscordListener) provider.GetRequiredService(listener)).Setup();
                await logger.Log($"{listener.Name} is set up", Discord.LogSeverity.Info);

            }

            return provider;
        }
    }
}