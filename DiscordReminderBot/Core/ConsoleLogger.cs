using Discord;
using DiscordReminderBot.Interfaces;


namespace DiscordReminderBot.Core
{
    public class ConsoleLogger : IDiscordLogger
    {
        private object _lock = new object();
        public Task Log(LogMessage message)
        {
            return Log(message.ToString(), message.Severity);
        }

        public Task Log(string message, LogSeverity severity)
        {
            // lock to prevent race conditions when changing colors of console
            lock (_lock)
            {
                if (severity >= LogSeverity.Info)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message);
                }
                else if (severity == LogSeverity.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
