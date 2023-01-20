using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;
using ServerAPI.Services.FileManager;
using System;
using System.Net;
using System.Net.Mail;

namespace ServerAPI.Controllers
{
    [Route("Files")]
    public class FilesController: ControllerBase
    {
        private readonly IFileManager fileManager;

        public FilesController(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        [HttpPost]
        public async Task<string> LoadFiles(IFormFileCollection formFiles)
        {
            var directoryId = fileManager.CreateDirectory();

            foreach (var file in formFiles)
            {
                using(var fileUploadStream = file.OpenReadStream())
                {
                    await fileManager.AddFile(fileUploadStream, directoryId, file.FileName);
                }
            }

            return directoryId;
        }

        [Route("{directoryId}"), HttpGet]
        public async Task<IEnumerable<string>> GetFileNames(string directoryId)
        {
            return await Task.Run(() =>
            {
                return fileManager.GetFileNames(directoryId);
            });
        }

        [Route("directoryId/file"), HttpGet]
        public async Task GetFile(string directoryId, string fileName)
        {
            var fileInfo = fileManager.GetFile(directoryId, fileName);
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

            await Response.SendFileAsync(fileInfo);
        }
    }
}
