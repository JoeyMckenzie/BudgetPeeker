using System.Diagnostics;
using BudgetPeeker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetPeeker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //
        // Will add these a little later, my eyes hurt
//        public IActionResult About()
//        {
//            return View();
//        }
//
//        public IActionResult Contact()
//        {
//            return View();
//        }
//
//        public IActionResult Privacy()
//        {
//            return View();
//        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}