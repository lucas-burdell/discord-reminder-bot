using DiscordReminderBot.Models;

namespace DiscordReminderBot.Interfaces
{
    internal interface IUserTimerService
    {
        public Task<UserTimer?> GetUserTimer(ulong userId);
        public Task SetUserTimer(ulong userId, UserTimer userTimer);
        public Task CheckAllTimers();
    }
}