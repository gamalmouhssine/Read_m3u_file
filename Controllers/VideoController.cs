using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Read_m3u_file.Helper;

namespace Read_m3u_file.Controllers
{
    public class VideoController : Controller
    {
        public ActionResult Playlist()
        {
            string m3uFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "family.m3u");

            if (!System.IO.File.Exists(m3uFilePath))
            {
                return NotFound("Playlist file not found.");
            }

            var videoUrls = M3UHelper.ReadM3UFile(m3uFilePath);

            return View(videoUrls);
        }
    }
}
