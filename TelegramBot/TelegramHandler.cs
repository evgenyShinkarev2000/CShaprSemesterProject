using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Archivator;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.Stickers;
using TelegramBot;
namespace TelegramBot
{
    public class TelegramHandler
    {
        public event Action<string> Notify;
        private string token;
        private int errorsCount;
        private BotClient api;
        private IArchivator archivator;
        public TelegramHandler(string token, IArchivator archivator)
        {
            this.token = token;
            api = new BotClient(token);
            this.archivator = archivator;
        }

        public async void Execute()
        {
            while (true)
            {
            var updates = api.GetUpdates();
                if (updates.Any())
                {
                    foreach (var update in updates)
                    {
                        Notify += (message) =>
                        {
                            api.SendMessage(update.Message.Chat.Id, message);
                        };
                        if (update.Message.Text == "/start")
                            api.SendMessage(update.Message.Chat.Id,
                                "Закидывай файлы в бота, напиши /end, когда закончишь");
                        if (update.Message.Document != null || update.Message.Photo != null
                            || update.Message.Audio != null || update.Message.Video != null)
                             Process(update.Message);
                        
                        if (update.Message.Text == "/end")
                        {
                            api.SendMessage(update.Message.Chat.Id, "Введи имя архива:");
                            var target = api.GetUpdates().First().Message.Text;
                            SendCompressed(update.Message, target);
                        }
                    }
                }
            }
        }

        private async void Process(Message message)
        {
            var id = message.Chat.Id;
            if (!Directory.Exists(id.ToString()))
                Directory.CreateDirectory(id.ToString());
            if (message.Document != null)
                DonwloadFile(new(message.Document, message.Document.FileName),
           $"{message.Chat.Id}\\{message.Document.FileName}");

            else if (errorsCount != 0)
            {
                api.SendMessageAsync(message.Chat.Id, "Отправьте пожалуйста документом!");
                errorsCount++;
            }
            else errorsCount = 0;
        }

        private  void DonwloadFile(TelegramFileInfo file, string destinationPath)
        {
            var id = file.telegramBase.FileId;
            var info =  api.GetFile(id);
                Notify($"Файл {info.FilePath} грузится");
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(
                    new Uri($"https://api.telegram.org/file/bot{token}/{info.FilePath})"),
                    destinationPath);
            }
            Notify("Файл загружен");
        }

        private void SendCompressed(Message message, string target)
        {
            var files = Directory.GetFiles(message.Chat.Id.ToString());
            foreach (var file in files)
            {
                archivator.AddToArchive(file);
            }
            archivator.Compress($"{message.Chat.Id}\\{target}");
            using (MemoryStream stream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream($"{message.Chat.Id}\\{target}", FileMode.Open))
                    fileStream.CopyTo(stream);
                api.SendDocumentAsync(message.Chat.Id, new InputFile(stream, target));
            }

            

            
        }
    }
}
