using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Archivator;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
    public class TelegramHandler
    {
        private string token;
        private TelegramBotClient botClient;
        private IArchivator archivator;
        private bool areFilesUploaded;
        public TelegramHandler(IArchivator archivator, string token)
        {
            this.archivator = archivator;
            this.token = token;
            this.botClient = new TelegramBotClient(token);
        }

        public void Execute()
        {
            botClient.StartReceiving(OnUpdate, OnError);
        }

        private async Task OnError(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
             Console.WriteLine(exception.StackTrace+"\n"+exception);
            return;
        }

        private async Task OnUpdate(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            
            if (areFilesUploaded)
            {
                if (message.Text.EndsWith(".zip"))
                    await SendArchive(message.Chat.Id.ToString(), 
                        $"{message.Chat.Id}\\{message.Text}", message.Text);
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Архив готовится!");
                return;
            }
            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Кидай мне файлы, как закончишь, пиши /end");
                return;
            }

            else if (message.Text == "/end")
            {
                areFilesUploaded = true;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите имя архива:");
            }
            else GetFile(update.Message);
        }

        private async Task SendArchive(string id, string path, string outFileName)
        {
            foreach (var file in Directory.EnumerateFiles(id.ToString()))
                archivator.AddToArchive(file);
            archivator.Compress(path);
            using Stream stream = System.IO.File.OpenRead(path);
            var document = new InputOnlineFile(stream, outFileName);
           var task = botClient.SendDocumentAsync(id, document);
            await task;
            if (task.IsCompletedSuccessfully)
            {
                stream.Close();
                Directory.Delete(id, true);
                areFilesUploaded = false;
               await botClient.SendTextMessageAsync(id,
                    "Можно делать следующий архив, как закинешь файлы, пиши /end");

            }
            return;
        }

        private void GetFile(Message message)
        {
            if (message.Document != null)
            {

                DownloadFile(message.Document.FileId, message.Document.FileName,
                    message.Chat.Id.ToString());
            }
            else if (message.Photo != null)
            {
                var file = message.Photo[message.Photo.Length - 1];
                var name = file.FileUniqueId.ToString() + ".jpg";
                var chatId = message.Chat.Id.ToString();
                DownloadFile(file.FileId, name, chatId);
            }
            else if (message.Audio!=null)
            {
                DownloadFile(message.Audio.FileId, message.Audio.FileName,
                    message.Chat.Id.ToString());
            }
            else if (message.Video!=null)
            {
                DownloadFile(message.Video.FileId, message.Video.FileName, message.Chat.Id.ToString());
            }
        }

        private void DownloadFile(string fileId, string fileName, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
         using (var fileStream = new FileStream($"{directory}\\{fileName}", FileMode.OpenOrCreate))
            {
                var info = botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
                Console.WriteLine(info.Result.FileSize);
            }
        }
    }
}
