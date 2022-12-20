using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator
{
    public class DefaultArchivator : IArchivator
    {
        public void Compress(string sourceRoute, string targetRoute)
        {
            using (FileStream fileStream = File.Create(targetRoute))
            using (var zipOutStream = new ZipOutputStream(fileStream))
            {

                //0-9, 9 being the highest level of compression
                zipOutStream.SetLevel(3);

                // This setting will strip the leading part of the folder path in the entries, 
                // to make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign to 0.
                int folderOffset = sourceRoute.Length + (sourceRoute.EndsWith("\\") ? 0 : 1);

                CompressFileOrDirectory(sourceRoute, zipOutStream, folderOffset);
            }
        }

        private void CompressFileOrDirectory(string sourceRoute, ZipOutputStream zipOutputStream, int folderOffset)
        {
            if (File.Exists(sourceRoute))
            {
                CompressFile(new FileInfo(sourceRoute), zipOutputStream, "");
            }
            else if (Directory.Exists(sourceRoute))
            {
                var directoryInfo = new DirectoryInfo(sourceRoute);
                CompressDirectory(directoryInfo, zipOutputStream, directoryInfo.Name);
            }
            else
            {
                throw new Exception("Путь не существует");
            }

            //var files = Directory.GetFiles(sourceRoute);

            //foreach (var filename in files)
            //{

            //    var fileInfo = new FileInfo(filename);

            //    // Make the name in zip based on the folder
            //    var entryName = filename.Substring(folderOffset);

            //    // Remove drive from name and fix slash direction
            //    entryName = ZipEntry.CleanName(entryName);

            //    var newZipEntry = new ZipEntry(entryName);

            //    // Note the zip format stores 2 second granularity
            //    newZipEntry.DateTime = fileInfo.LastWriteTime;

            //    // Specifying the AESKeySize triggers AES encryption. 
            //    // Allowable values are 0 (off), 128 or 256.
            //    // A password on the ZipOutputStream is required if using AES.
            //    //   newEntry.AESKeySize = 256;

            //    // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
            //    // WinZip 8, Java, and other older code, you need to do one of the following: 
            //    // Specify UseZip64.Off, or set the Size.
            //    // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, 
            //    // you do not need either, but the zip will be in Zip64 format which
            //    // not all utilities can understand.
            //    //   zipStream.UseZip64 = UseZip64.Off;
            //    newZipEntry.Size = fileInfo.Length;

            //    zipOutputStream.PutNextEntry(newZipEntry);

            //    // Zip the file in buffered chunks
            //    // the "using" will close the stream even if an exception occurs
            //    var buffer = new byte[4096];
            //    using (FileStream fsInput = File.OpenRead(filename))
            //    {
            //        StreamUtils.Copy(fsInput, zipOutputStream, buffer);
            //    }
            //    zipOutputStream.CloseEntry();
            //}

            //// Recursively call CompressFolder on all folders in path
            //var folders = Directory.GetDirectories(sourceRoute);
            //foreach (var folder in folders)
            //{
            //    CompressFileOrDirectory(folder, zipOutputStream, folderOffset);
            //}
        }

        private void CompressFile(FileInfo fileInfo, ZipOutputStream zipOutputStream, string directoryPath)
        {
            // Make the name in zip based on the folder
            var entryName = directoryPath + fileInfo.Name;

            // Remove drive from name and fix slash direction
            entryName = ZipEntry.CleanName(entryName);

            var newZipEntry = new ZipEntry(entryName);

            // Note the zip format stores 2 second granularity
            newZipEntry.DateTime = fileInfo.LastWriteTime;

            // Specifying the AESKeySize triggers AES encryption. 
            // Allowable values are 0 (off), 128 or 256.
            // A password on the ZipOutputStream is required if using AES.
            //   newEntry.AESKeySize = 256;

            // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
            // WinZip 8, Java, and other older code, you need to do one of the following: 
            // Specify UseZip64.Off, or set the Size.
            // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, 
            // you do not need either, but the zip will be in Zip64 format which
            // not all utilities can understand.
            //   zipStream.UseZip64 = UseZip64.Off;
            newZipEntry.Size = fileInfo.Length;

            zipOutputStream.PutNextEntry(newZipEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            var buffer = new byte[4096];
            using (FileStream fsInput = File.OpenRead(fileInfo.FullName))
            {
                StreamUtils.Copy(fsInput, zipOutputStream, buffer);
            }
            zipOutputStream.CloseEntry();
        }

        private void CompressDirectory(DirectoryInfo directoryInfo, ZipOutputStream zipOutputStream, string directoryPath)
        {
            var isDirectoryEmpty = true;
            foreach (var file in directoryInfo.GetFiles())
            {
                isDirectoryEmpty = false;
                CompressFile(file, zipOutputStream, directoryPath + "\\");
            }
            foreach(var directory in directoryInfo.GetDirectories())
            {
                isDirectoryEmpty= false;
                CompressDirectory(directory, zipOutputStream, directoryPath + "\\" + directory.Name);
            }

            if (isDirectoryEmpty)
            {
                var folderZipEntry = new ZipEntry(ZipEntry.CleanName(directoryPath + "\\"));
                zipOutputStream.PutNextEntry(folderZipEntry);
                zipOutputStream.CloseEntry();
            }
        }

        public void Decompress(string compressedDirectoryRoute, string targetOut)
        {
            using (var fsInput = File.OpenRead(compressedDirectoryRoute))
            using (var zf = new ZipFile(fsInput))
            {

                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:
                    //entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(targetOut, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }
    }
}
