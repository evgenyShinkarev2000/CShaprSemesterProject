// See https://aka.ms/new-console-template for more information
using Archivator;
using TelegramBot;

var handler = new TelegramHandler(new DefaultArchivator(),
    "5964650656:AAG6ruF9XW_eFIxSaTfJha2OEeQQ_cCA7YM");
handler.Execute();
Console.ReadLine();
