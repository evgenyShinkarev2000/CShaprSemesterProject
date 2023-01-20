using Microsoft.Extensions.FileProviders;

namespace ServerAPI.Services.ArchiveManager
{
    public interface IArchiveManager
    {
        public IFileInfo GetOrCreateArchiveById(string id);
    }
}
