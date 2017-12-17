using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SGGWPZ.Models;
using SGGWPZ.Repositories;

namespace SGGWPZ.Controllers
{
    public class HomeController : Controller
    {
        private PlanContext _db;
        IUniversalRepositoryTypeOf uni;

        public HomeController(PlanContext db, IUniversalRepositoryTypeOf UNI)
        {
            _db = db;
            uni = UNI;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            var x = _db;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            //var o = uni.Obiekt("Grupy");
            //var obiekty = uni.ReadAllT(o);

            return View();
        }

        public IActionResult Error()
        {
            return View();//new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
