using FileChecks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileChecks.Controllers
{
    public class FolderCheckController : Controller
    {
        private readonly IVersionManagerFactory _factory;
        public FolderCheckController(IVersionManagerFactory factory)
        {
            _factory = factory;
        }

        //[HttpPost]
        //public IActionResult Open()
        //{
        //    string path = @"C:\MyFolder";

        //    Process.Start(new ProcessStartInfo
        //    {
        //        FileName = "explorer.exe",
        //        Arguments = path,
        //        UseShellExecute = true
        //    });

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public IActionResult Index(string folderPath)
        {
            var versionManager = _factory.Create();

            if (ModelState.IsValid)
            {
                versionManager.Start(folderPath);
            }

            return View(versionManager);
        }

        public IActionResult Index()
        {
            var versionManager = _factory.Create();
            versionManager.Start(string.Empty);

            return View(versionManager);
        }
    }
}
