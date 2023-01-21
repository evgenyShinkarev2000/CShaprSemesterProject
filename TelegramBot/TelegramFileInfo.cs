using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;

namespace TelegramBot
{
    public class TelegramFileInfo
    {
        public readonly TelegramFileBase telegramBase;
        public readonly string Name;

        public TelegramFileInfo(TelegramFileBase telegramBase, string name)
        {
            this.telegramBase = telegramBase;
            Name = name;
        }
    }
}
