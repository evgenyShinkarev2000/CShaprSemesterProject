// See https://aka.ms/new-console-template for more information
using Archivator;
using TelegramBot;

var tgh = new TelegramHandler("5967892030:AAEeJ-PMg7RppglVUOtI7PHSCAxaQ4vaHno", new DefaultArchivator());
while (true)
tgh.Execute();
