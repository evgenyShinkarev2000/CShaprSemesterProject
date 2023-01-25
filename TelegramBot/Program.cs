// See https://aka.ms/new-console-template for more information
using Archivator;
using TelegramBot;

var handler = new TelegramHandler(new DefaultArchivator(),
    "token");
handler.Execute();
Console.ReadKey();
