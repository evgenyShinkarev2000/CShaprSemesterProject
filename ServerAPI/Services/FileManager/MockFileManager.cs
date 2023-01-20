using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.FileProviders;

namespace ServerAPI.Services.FileManager
{
    public class MockFileManager : IFileManager
    {
        public static readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks", "Uploads");

        public MockFileManager()
        {
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }

            Directory.CreateDirectory(basePath);
        }

        public IFileInfo GetFile(string directoryId, string fileName)
        {
            return new PhysicalFileInfo(new FileInfo(Path.Combine(basePath, directoryId, fileName)));
        }

        public IEnumerable<string> GetFileNames(string directoryId)
        {
            return Directory.GetFiles(Path.Combine(basePath, directoryId)).Select(fullName => Path.GetFileName(fullName));
        }

        public string CreateDirectory()
        {
            var guid = Convert.ToHexString(Guid.NewGuid().ToByteArray());
            Directory.CreateDirectory(Path.Combine(basePath, guid));

            return guid;
        }

        public async Task AddFile(Stream readStream, string directoryId, string fileName)
        {
            using (var fileStream = File.Create(Path.Combine(basePath, directoryId, fileName)))
            {
                await readStream.CopyToAsync(fileStream);
            }
        }

        public string GetPathToDirectory(string directoryId) => Path.Combine(basePath, directoryId);
    }
}
