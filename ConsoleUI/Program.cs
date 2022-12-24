﻿using Archivator;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;

namespace ConsoleUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var archivator = new DefaultArchivator();
            var sourceRoute = @"C:\projects\CSharp\CShaprSemesterProject\ConsoleUI\samples\Sources";
            var archivesRoute = @"C:\projects\CSharp\CShaprSemesterProject\ConsoleUI\samples\Archives\FirstArchive.zip";
            var decompressedRoute = @"C:\projects\CSharp\CShaprSemesterProject\ConsoleUI\samples\Decompressed";
            archivator.Compress(sourceRoute, archivesRoute);
            archivator.Decompress(new ZipFile(File.OpenRead(archivesRoute)), decompressedRoute);
        }
    }
}