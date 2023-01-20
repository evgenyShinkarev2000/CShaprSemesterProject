using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace ServerAPI.Services.FileManager
{
    public interface IFileManager
    {
        private static readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks", "Uploads");
        public IFileInfo GetFile(string directoryId, string fileName);
        public IEnumerable<string> GetFileNames(string directoryId);
        public string CreateDirectory();
        public Task AddFile(Stream readStream, string directoryId, string fileName);
        public string GetPathToDirectory(string directoryId);
    }
}
