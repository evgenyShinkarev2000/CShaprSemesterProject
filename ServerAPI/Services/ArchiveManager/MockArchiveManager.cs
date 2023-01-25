using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using ServerAPI.Services.FileManager;
using Archivator;

namespace ServerAPI.Services.ArchiveManager
{
    public class MockArchiveManager : IArchiveManager
    {
        public static readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks", "Archives");
        private readonly IFileManager fileManager;

        public MockArchiveManager(IFileManager fileManager)
        {
            this.fileManager = fileManager;
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }

            Directory.CreateDirectory(basePath);
        }

        public IFileInfo GetOrCreateArchiveById(string id)
        {
            var archivePath = Path.Combine(basePath, id);
            if (!Directory.Exists(archivePath))
            {
                var archivator = new Archivator.DefaultArchivator();
                archivator.AddToArchive(fileManager.GetPathToDirectory(id));
                archivator.Compress(Path.Combine(basePath, id));
            }

            return new PhysicalFileInfo(new FileInfo(archivePath));
        }
    }
}
