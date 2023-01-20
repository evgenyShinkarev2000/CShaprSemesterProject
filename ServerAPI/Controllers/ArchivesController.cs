using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Services.ArchiveManager;
using ServerAPI.Services.FileManager;

namespace ServerAPI.Controllers
{
    [Route("Archives")]
    public class ArchivesController : Controller
    {
        private readonly IFileManager fileManager;
        private readonly IArchiveManager archiveManager;

        public ArchivesController(IFileManager fileManager, IArchiveManager archiveManager)
        {
            this.fileManager = fileManager;
            this.archiveManager = archiveManager;
        }

        [Route("{archiveId}"), HttpGet]
        public async Task GetArchive(string archiveId)
        {
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + archiveId + ".zip");
            var fileInfo = archiveManager.GetOrCreateArchiveById(archiveId);

            await Response.SendFileAsync(fileInfo);
        }

        [HttpPost]
        public async Task<string> CompressFile(IFormFileCollection formFiles)
        {
            var directoryId = fileManager.CreateDirectory();

            foreach (var file in formFiles)
            {
                using (var fileUploadStream = file.OpenReadStream())
                {
                    await fileManager.AddFile(fileUploadStream, directoryId, file.FileName);
                }
            }

            archiveManager.GetOrCreateArchiveById(directoryId);

            return directoryId;
        }
    }
}
