using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordReminderBot.Models
{
    internal class UserTimer
    {
        public ulong UserID { get; set; }
        public ulong ChannelID { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}
