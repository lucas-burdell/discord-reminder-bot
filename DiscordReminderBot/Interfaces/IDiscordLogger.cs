using Discord;

namespace DiscordReminderBot.Interfaces
{
    public interface IDiscordLogger
    {
        Task Log(LogMessage message);
        Task Log(string message, LogSeverity severity);
    }
}