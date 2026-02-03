using System.Diagnostics;
using FileChecks.Models;
using FileChecks.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FileChecks.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVersionManagerFactory _factory;
        public HomeController(IVersionManagerFactory factory)
        {
            _factory = factory;
        }

        [HttpPost]
        public IActionResult Index(string? subFolderPath)
        {
            var versionManager = _factory.Create();

            if (ModelState.IsValid)
            {
                versionManager.Start(subFolderPath);
            }

            return View(versionManager);
        }

        public IActionResult Index()
        {
            var versionManager = _factory.Create();
            //versionManager.Start(string.Empty);

            return View(versionManager);
        }
    }
}
