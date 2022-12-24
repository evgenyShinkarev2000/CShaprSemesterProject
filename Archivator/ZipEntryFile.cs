using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator
{
    public class ZipEntryFile
    {
        public ZipEntry ZipEntry { get; private set; }
        public FileInfo? FileInfo { get; private set; }

        public ZipEntryFile(ZipEntry zipEntry, FileInfo? fIleInfo = null)
        {
            FileInfo = fIleInfo;
            ZipEntry = zipEntry;
        }
    }
}
