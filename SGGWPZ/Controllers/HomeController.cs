using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View();//new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Zaloguj()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Zaloguj(Uzytkownik uzytkownik)
        {
            var konto = _db.Uzytkownik.Where(u => u.login == uzytkownik.login && u.haslo == uzytkownik.haslo).FirstOrDefault();
            if (konto != null)
            {
                HttpContext.Session.SetString("uzytkownikId", konto.uzytkownikId.ToString());
                HttpContext.Session.SetString("login", konto.login);

                var rk = await uni.ReadTAsync(konto.rodzajuzytkownikaId, uni.Obiekt("Rodzaj_uzytkownika"));
                string rodzkonta = rk.GetType().GetProperty("rodzajuzytkownika").GetValue(rk);
                HttpContext.Session.SetString("rodzaj_konta", rodzkonta);
            }
            else
            {
                ModelState.AddModelError("", "Podane dane do logowania są niepoprawne!");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Wyloguj()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
