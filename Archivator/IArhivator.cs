using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator
{
    public interface IArchivator
    {
        public void Compress(string sourceRoute, string targetRoute);
        public void Compress(IEnumerable<ZipEntryFile> zipEntryFiles, ZipOutputStream zipOutputStream);
        public void Decompress(ZipFile zipFile, string outDirectoryPath);
    }
}
