using AspNetCore.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFileProvider fileProvider;
        private readonly IConfiguration configuration;
        public HomeController(ILogger<HomeController> logger, IFileProvider fileProvider, IConfiguration configuration)
        {
            _logger = logger;
            this.fileProvider = fileProvider;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.MySqlCon = this.configuration["MySqlCon"];
            return View();
        }

        public IActionResult ImageShow()
        {
            var images = this.fileProvider.GetDirectoryContents("wwwroot/Images")
                .ToList().Select(x => x.Name);

            return View(images);
        }

        [HttpPost]
        public IActionResult ImageShow(string name)
        {
            var file = this.fileProvider.GetDirectoryContents("wwwroot/Images")
                .ToList().First(x => x.Name == name);

            System.IO.File.Delete(file.PhysicalPath);
            return RedirectToAction(nameof(ImageShow));
        }
        public IActionResult ImageSave()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImageSave(IFormFile imageFile)
        {
            if (imageFile is { Length: > 0 } && imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() +
                    Path.GetExtension(imageFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }
            return View();
        }   


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
