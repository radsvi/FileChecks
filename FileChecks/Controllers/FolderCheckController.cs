using FileChecks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileChecks.Controllers
{
    public class FolderCheckController : Controller
    {
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
            var versionManager = new VersionManager(folderPath);

            if (ModelState.IsValid)
            {
                versionManager.Start();
            }

            return View(versionManager);
        }

        public IActionResult Index()
        {
            var versionManager = new VersionManager(string.Empty);
            versionManager.Start();

            return View(versionManager);
        }
    }
}
