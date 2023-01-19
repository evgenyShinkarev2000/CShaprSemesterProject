using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Archivator
{
    public class TarArchivator : IArchivator
    {
       

        public void Compress(string sourceRoute, string targetRoute)
        {
             CreateTarGz(sourceRoute, targetRoute);
            
        }

        private void CreateTarGz(string sourceRoute, string targetRoute)
        {
            Stream outStream = File.Create(targetRoute);
            Stream gzoStream = new GZipOutputStream(outStream);
            TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

           
            tarArchive.RootPath = sourceRoute.Replace('\\', '/');
            if (tarArchive.RootPath.EndsWith("/"))
                tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

            AddDirectoryFilesToTar(tarArchive, sourceRoute, true);

            tarArchive.Close();
        }

        private void AddDirectoryFilesToTar
            (TarArchive tarArchive, string sourceRoute, bool recursive)
        {
            TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceRoute);
            tarArchive.WriteEntry(tarEntry, false);

            
            string[] filenames = Directory.GetFiles(sourceRoute);
            foreach (string filename in filenames)
            {
                tarEntry = TarEntry.CreateEntryFromFile(filename);
                tarArchive.WriteEntry(tarEntry, true);
            }

            if (recursive)
            {
                string[] directories = Directory.GetDirectories(sourceRoute);
                foreach (string directory in directories)
                    AddDirectoryFilesToTar(tarArchive, directory, recursive);
            }
        }

        public void Decompress(string sourceRoute, string outDirectoryPath)
        {
            
        }
    }
}
