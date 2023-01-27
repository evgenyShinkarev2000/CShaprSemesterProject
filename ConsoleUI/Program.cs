using Archivator;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Data;

namespace ConsoleUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Expected min 2 parameters. Command and path");
                return;
            }
            var command = args[0];
            var path = args[1];
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine("Cant find file or directory in " + path);
                return;
            }
            var archivator = new DefaultArchivator();
            switch (command)
            {
                case "pack":
                    archivator.Compress(
                        path,
                        Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + ".zip"));
                    break;
                case "unpack":
                    archivator.Decompress(path, Path.GetDirectoryName(path)!);
                    break;
                default:
                    Console.WriteLine("Unknown command " + command);
                    return;
            }   
        }
    }
}