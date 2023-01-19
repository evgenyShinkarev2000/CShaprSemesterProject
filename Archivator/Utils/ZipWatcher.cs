using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Archivator.Utils
{
    public class ZipWatcher
    {
        private ZipInputStream archive;
        public ZipWatcher(string sourceFile)
        {
            archive = new ZipInputStream(File.OpenRead(sourceFile));
        }

        public IEnumerable<ZipEntry> WatchFiles()
        {
            while (true)
            {
                var entry = archive.GetNextEntry();
                if (entry == null)
                    yield break;
                yield return entry;
            }
        }
    }
}
