using Discord;
using Discord.WebSocket;
using DiscordReminderBot.Interfaces;

namespace DiscordReminderBot.Core
{
    internal class Server
    {
        private DiscordSocketClient _client;
        private readonly IDiscordLogger _logger;
        private readonly BotConfig _config;

        internal Server(BotConfig config, IDiscordLogger logger, DiscordSocketClient client)
        {
            _config = config;
            _logger = logger;   
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _logger.Log("Starting server...", LogSeverity.Info);
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite, cancellationToken);
            await _logger.Log("Stopping server...", LogSeverity.Info);
            await _client.StopAsync();
        }
    }
}
