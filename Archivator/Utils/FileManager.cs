using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator.Utils
{
    public class FileManager
    {
        private IArchivator archivator;
        private string rootDirectoryName;
        private List<string> selectedFiles;

        public FileManager(IArchivator archivator, string rootDirectoryName)
        {
            this.archivator = archivator;
            this.rootDirectoryName = rootDirectoryName;
            selectedFiles = new List<string>();
        }
        public IEnumerable<string> GetFilesAndDirectories()
        {
            foreach (var name in Directory.GetFileSystemEntries(rootDirectoryName))
                yield return name;
        }
    }
}
