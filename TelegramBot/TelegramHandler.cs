using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Archivator;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
    public class TelegramHandler
    {
        private IArchivator archivator;
        private string token;
        private TelegramBotClient client;
        private bool areFilesUploaded;

        public TelegramHandler(IArchivator archivator, string token)
        {
            this.archivator = archivator;
            this.token = token;
            client = new TelegramBotClient(token);
        }
        public void Execute()
        {
            client.StartReceiving(UpdateHandler, ExceptionHandler);
        }

        private async Task ExceptionHandler(ITelegramBotClient client, Exception exception,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update,
            CancellationToken token)
        {
            var message = update.Message;
            var id = message.Chat.Id.ToString();
            if (areFilesUploaded)
            {
                var fileName = message.Text.EndsWith(".zip") ? message.Text :
                    $"{message.Text}.zip";
                var path = $"{id}\\{fileName}";
                SendArchive(id, path, fileName);
                
                return;
            }
            else if (message.Text == "/start")
               await client.SendTextMessageAsync(id,
                    "Закидывай в меня файлы, как закончишь, пиши /end");
            else if (message.Text == "/end")
            {
                areFilesUploaded = true;
                await client.SendTextMessageAsync(id,
                   "Введите имя архива");
            }
            else if (message.Document != null)
                DownloadFile(message.Document.FileId, message.Document.FileName, id);
            else if (message.Audio != null)
                DownloadFile(message.Audio.FileId, message.Audio.FileName, id);
            else if (message.Photo != null)
                DownloadPhoto(message.Photo, id);
        }

        private void DownloadPhoto(PhotoSize[] photo, string id)
        {
            var fileId = photo[photo.Length - 1].FileId;
            DownloadFile(fileId, fileId, id);
        }

        private void DeleteTempFiles(string id)
        {
            Directory.Delete(id, true);
        }

        private async void DownloadFile(string fileId, string fileName, string id)
        {
            if (!Directory.Exists(id))
                Directory.CreateDirectory(id);
            using (var stream = new FileStream($"{id}\\{fileName}", FileMode.OpenOrCreate))
            {
                var info = await client.GetFileAsync(fileId);
                var path = info.FilePath;
                await client.DownloadFileAsync(path, stream);
            }
        }

        private async Task SendArchive(string id, string path, string fileName)
        {
            foreach (var file in Directory.EnumerateFiles(id))
            {
                archivator.AddToArchive(file);
            }

            archivator.Compress(path);
            using (var stream = System.IO.File.OpenRead(path))
            {
                var document = new InputOnlineFile(stream, fileName);
                await client.SendDocumentAsync(id, document);
            }
            var sending = client.SendTextMessageAsync(id,
                "Можешь создавать новый архив по окончании пиши /end");
            await sending;
            if (sending.IsCompletedSuccessfully)
                DeleteTempFiles(id);
            areFilesUploaded = false;
        }
    }
}
