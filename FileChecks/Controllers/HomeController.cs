using System.Diagnostics;
using FileChecks.Models;
using FileChecks.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FileChecks.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
