using Discord;
using Discord.WebSocket;
using DiscordReminderBot.Interfaces;
using DiscordReminderBot.Models;

namespace DiscordReminderBot.Listeners
{
    internal class DMMessageListener : IDiscordListener
    {
        private readonly DiscordSocketClient _client;
        private readonly IDiscordLogger _logger;
        private readonly IUserTimerService _userTimerService;
        private readonly Emoji reaction = new Emoji("⏰");

        public DMMessageListener(DiscordSocketClient client, IDiscordLogger logger, IUserTimerService userTimerService) {
            _client = client;
            _logger = logger;
            _userTimerService = userTimerService;
        }

        public void Setup()
        {
            _client.MessageReceived += Listen;
        }

        public async Task Listen(SocketMessage message)
        {
            IChannel channel = message.Channel;
            if (channel == null)
            {
                await _logger.Log($"Received message did not have an associated channel", LogSeverity.Warning);
                return;
            }
            if (channel.GetChannelType() != ChannelType.DM)
            {
                await _logger.Log($"Received message was not from a DM channel", LogSeverity.Warning);
                return;
            }
            var user = (await AsyncEnumerableExtensions.FlattenAsync(channel.GetUsersAsync(CacheMode.AllowDownload))).Where(channelUser => !channelUser.IsBot).First();


            var existingTimer = await _userTimerService.GetUserTimer(user.Id);
            if (existingTimer != null)
            {
                await message.AddReactionAsync(reaction);
                return;
            }

            var userTimer = new UserTimer()
            {
                UserID = user.Id,
                ChannelID = channel.Id,
                TimeCreated = DateTime.UtcNow
            };

            await _userTimerService.SetUserTimer(user.Id, userTimer);


            await message.AddReactionAsync(reaction);
        }
    }
}
