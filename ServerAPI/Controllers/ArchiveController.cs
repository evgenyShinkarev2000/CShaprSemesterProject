using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers
{
    [Route("Archive")]
    public class ArchiveController: ControllerBase
    {
        [HttpPost]
        public async Task<string> LoadFiles()
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2000);

                return "проснулся";
            });
        }
    }
}
