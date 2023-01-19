using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator
{
    public class DefaultArchivator : IArchivator
    {
        public IEnumerable<ZipEntryFile> BuildAcrhiveTree(string sourceRoute)
        {
            if (File.Exists(sourceRoute))
            {
                yield return BuildFileZipEntry(new FileInfo(sourceRoute), "");
            }
            else if (Directory.Exists(sourceRoute))
            {
                var directoryInfo = new DirectoryInfo(sourceRoute);
                foreach(var zipEntry in BuildDirectoryZipEntries(directoryInfo, directoryInfo.Name))
                {
                    yield return zipEntry;
                }
            }
        }

        private ZipEntryFile BuildFileZipEntry(FileInfo fileInfo, string prefixPath)
        {
            var entryLocalPath = Path.Combine(prefixPath, fileInfo.Name);
            var zipEntry = new ZipEntry(ZipEntry.CleanName(entryLocalPath))
            {
                DateTime = fileInfo.LastWriteTime,
                Size = fileInfo.Length,
            };

            return new ZipEntryFile(zipEntry, fileInfo);
        }

        private IEnumerable<ZipEntryFile> BuildDirectoryZipEntries(DirectoryInfo directoryInfo, string prefixPath)
        {
            var files = directoryInfo.EnumerateFiles();
            var directories = directoryInfo.EnumerateDirectories();
            if (!files.Any() && ! directories.Any()) 
            {
                yield return new ZipEntryFile(new ZipEntry(prefixPath + Path.DirectorySeparatorChar));
                yield break;
            }

            foreach (var file in files)
            {
                yield return BuildFileZipEntry(file, prefixPath + Path.DirectorySeparatorChar);
            }

            foreach (var directory in directories)
            {
                foreach (var zipEntry in BuildDirectoryZipEntries(directory, Path.Combine(prefixPath, directory.Name)))
                {
                    yield return zipEntry;
                }
            }
        }

        public void Decompress(string sourceFile, string outDirectoryPath)
        {
            var zipFile = new ZipFile(sourceFile);
            foreach (ZipEntry zipEntry in zipFile)
            {
                var targetPath = Path.Combine(outDirectoryPath, ZipEntry.CleanName(zipEntry.Name));
                var directoryPath = Path.GetDirectoryName(targetPath) ?? "";
                Directory.CreateDirectory(directoryPath);
                if (zipEntry.IsFile)
                {
                    using (var zipStream = zipFile.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(targetPath))
                    {
                        var buffer = new byte[4096];
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }

        public void Compress(string sourceRoute, string targetRoute)
        {
            if (!File.Exists(sourceRoute) && !Directory.Exists(sourceRoute)) 
            {
                throw new DirectoryNotFoundException($"Путь до архивируемых файлов {sourceRoute} не существует");
            }
            
            var zipEntries = BuildAcrhiveTree(sourceRoute);
            var prefixDirectories = Path.GetDirectoryName(targetRoute);

            if (prefixDirectories != null && prefixDirectories!="")
            {
                Directory.CreateDirectory(prefixDirectories);
            }

            using(var fileOutputStream = File.Create(targetRoute))
            using (var zipOutputStream = new ZipOutputStream(fileOutputStream))
            {
                Compress(zipEntries, zipOutputStream);
            }
        }

        public void Compress(IEnumerable<ZipEntryFile> zipEntryFiles, ZipOutputStream zipOutputStream)
        {
            foreach (var zipEntryFile in zipEntryFiles)
            {
                zipOutputStream.PutNextEntry(zipEntryFile.ZipEntry);
                if (zipEntryFile.ZipEntry.IsFile)
                {
                    var buffer = new byte[4096];
                    using (var fileStream = zipEntryFile.FileInfo!.OpenRead())
                    {
                        StreamUtils.Copy(fileStream, zipOutputStream, buffer);
                    }
                }
                zipOutputStream.CloseEntry();
            }
        }
    }
}
