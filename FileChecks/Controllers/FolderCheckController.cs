using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileChecks.Controllers
{
    public class FolderCheckController : Controller
    {
        [HttpPost]
        public IActionResult Open()
        {
            string path = @"C:\MyFolder";

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = path,
                UseShellExecute = true
            });

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
