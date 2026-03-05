using lps_crud_test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace lps_crud_test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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

        public IActionResult JenisBuku()
        {
            return View();
        }

        public IActionResult Buku()
        {
            return View();
        }
    }
}
