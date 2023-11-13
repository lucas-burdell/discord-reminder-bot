using DiscordReminderBot.Interfaces;
using DiscordReminderBot.Models;
using Discord;
using Discord.WebSocket;

namespace DiscordReminderBot.Services
{
    internal class UserTimerService : IUserTimerService, IAsyncDisposable
    {
        private readonly IDiscordLogger _logger;
        private readonly DiscordSocketClient _client;
        private Dictionary<ulong, UserTimer> _userTimers = new Dictionary<ulong, UserTimer>();
        private Timer _scheduleTimer;
        private readonly TimeSpan checkInterval = TimeSpan.FromHours(4);
        private readonly int hourOfDayUTCToAlert = 18;

        public UserTimerService(IDiscordLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
            _scheduleTimer = new Timer(x => Task.Run(
                async () => await CheckAllTimers()
            ), null, TimeSpan.FromMilliseconds(0), checkInterval);
        }

        public async Task CheckAllTimers()
        {
            await _logger.Log("Processing all user timers", LogSeverity.Info);
            foreach (var timer in _userTimers.Values)
            {
                if (DateTime.UtcNow >= GetNextDayAtAlertTime(timer.TimeCreated))
                {
                    try
                    {
                        IDMChannel channel = (IDMChannel) await _client.GetChannelAsync(timer.ChannelID);
                        if (channel != null)
                        {
                            await _logger.Log($"Sending message to user {timer.UserID} in channel {timer.ChannelID}", LogSeverity.Info);
                            await channel.SendMessageAsync("I am reminding you of the above messages.");
                            _userTimers.Remove(timer.UserID);
                        }
                        else
                        {
                            await _logger.Log($"Sending message to user {timer.UserID} in channel {timer.ChannelID} FAILED", LogSeverity.Error);
                        }
                    } catch (Exception e)
                    {
                        await _logger.Log(e.ToString(), LogSeverity.Error);
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _scheduleTimer.DisposeAsync();
        }

        public async Task<UserTimer?> GetUserTimer(ulong userId)
        {
            await _logger.Log($"Getting user timer for user {userId}", LogSeverity.Info);
            if (_userTimers.ContainsKey(userId))
            {
                return _userTimers[userId];
            } else
            {
                return null;
            }
        }
        public async Task SetUserTimer(ulong userId, UserTimer userTimer)
        {
            await _logger.Log($"Setting user timer for user {userId}", LogSeverity.Info);
            _userTimers[userId] = userTimer;
        }

        private DateTime GetNextDayAtAlertTime(DateTime time)
        {
            // if past the hour of day, alert the next day, otherwise alert same day
            var timeToAlert = time;
            if (time.Hour >= hourOfDayUTCToAlert)
            {
                timeToAlert = time.AddDays(1);
            }

            return timeToAlert.Date.AddHours(hourOfDayUTCToAlert);
        }
    }
}
