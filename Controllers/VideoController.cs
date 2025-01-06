using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Read_m3u_file.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Read_m3u_file.Controllers
{
    public class VideoController : Controller
    {
        private readonly ILogger<VideoController> _logger;

        public VideoController(ILogger<VideoController> logger)
        {
            _logger = logger;
        }

        // This method generates a unique user ID and stores it in a cookie.
        private string GetUserId()
        {
            if (!Request.Cookies.ContainsKey("UserId"))
            {
                var userId = Guid.NewGuid().ToString();
                Response.Cookies.Append("UserId", userId, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
                return userId;
            }
            return Request.Cookies["UserId"];
        }

        // This method creates a folder for each user to store their uploaded .m3u files.
        private string GetUserUploadFolder()
        {
            var userId = GetUserId();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userId);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            return uploadsFolder;
        }

        // This method deletes the selected .m3u file from the user's upload folder.
        public IActionResult DeleteM3UFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }
            var userUploadFolder = GetUserUploadFolder();
            var filePath = Path.Combine(userUploadFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }
            System.IO.File.Delete(filePath);
            return RedirectToAction("Index");
        }

        // This method retrieves the list of .m3u files uploaded by the user and displays them on the Index view.
        public IActionResult Index()
        {
            var userUploadFolder = GetUserUploadFolder();
            var files = Directory.GetFiles(userUploadFolder, "*.m3u")
                                 .Select(Path.GetFileName)
                                 .ToList();
            return View(files);
        }

        // This method handles the file upload process, checks if the uploaded file is a .m3u file, and saves it to the user's upload folder.
        [HttpPost("UploadM3UFile")]
        public IActionResult UploadM3UFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".m3u")
            {
                return BadRequest("Invalid file format. Please upload a .m3u file.");
            }

            try
            {
                var userUploadFolder = GetUserUploadFolder();
                var filePath = Path.Combine(userUploadFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // This method reads the selected .m3u file and displays the list of channels on the ViewChannels view.
        public IActionResult ViewChannels(string fileName, string filterBy = null, string sortBy = null, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            var userUploadFolder = GetUserUploadFolder();
            var filePath = Path.Combine(userUploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var channels = M3UHelper.ReadM3UFile(filePath);

            // Filtering
            if (!string.IsNullOrEmpty(filterBy))
            {
                channels = channels.Where(c => c.Name.Contains(filterBy, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        channels = channels.OrderBy(c => c.Name).ToList();
                        break;
                    case "name_desc":
                        channels = channels.OrderByDescending(c => c.Name).ToList();
                        break;
                }
            }

            // Pagination
            var totalItems = channels.Count;
            var paginatedChannels = channels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.FileName = fileName;
            ViewBag.FilterBy = filterBy;
            ViewBag.SortBy = sortBy;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(paginatedChannels);
        }
    }
}