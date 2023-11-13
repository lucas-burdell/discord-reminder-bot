using DiscordReminderBot.Interfaces;

namespace DiscordReminderBot.Core
{
    internal class LoggerFactory
    {
        private BotConfig _config;

        public LoggerFactory(BotConfig config)
        {
            _config = config;
        }

        public IDiscordLogger BuildLogger()
        {
            switch (_config.LogType)
            {
                case "console":
                    return new ConsoleLogger();
                default:
                    return new ConsoleLogger();
            }
        }
    }
}
