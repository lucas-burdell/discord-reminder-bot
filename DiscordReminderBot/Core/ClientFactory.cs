using Discord.WebSocket;
using DiscordReminderBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordReminderBot.Core
{
    internal class ClientFactory
    {
        private readonly BotConfig _config;
        private readonly IDiscordLogger _logger;

        public ClientFactory(BotConfig config, IDiscordLogger logger) {
            _config = config;
            _logger = logger;
        }

        public DiscordSocketClient BuildClient()
        {
            var client = new DiscordSocketClient();
            client.Log += _logger.Log;
            return client;
        }
    }
}
