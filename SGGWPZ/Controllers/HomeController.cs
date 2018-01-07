using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SGGWPZ.Models;
using SGGWPZ.Repositories;
using SGGWPZ.ViewModels;

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
            ViewPlanZajec vPZ = new ViewPlanZajec();
            vPZ.kierunek = "Informatyka";
            vPZ.semestr_studiow = "1";
            vPZ.grupa = "1";
            vPZ.datetime = DateTime.Now;

            try { vPZ.ZnajdzRezerwacje(uni); }
            catch (Exception ex) { }
            
            vPZ.PodzielRezerwacje(uni);
            vPZ.Uzupelanieniedanych(uni);
            vPZ.SprawdzDniWolne(uni);

            var test = vPZ.Rezerwacje;
            return View(vPZ);
        }

        [HttpPost]
        public IActionResult Index(ViewPlanZajec viewPlanZajec, int ile, string dt)
        {
            // Przesuniecie daty
            if (dt != null){ viewPlanZajec.datetime = Convert.ToDateTime(dt).AddDays(ile); }
            else           { viewPlanZajec.datetime = viewPlanZajec.datetime.AddDays(ile); }
            viewPlanZajec.datetimestring = viewPlanZajec.datetime.ToString();

            try { viewPlanZajec.ZnajdzRezerwacje(uni); }
            catch (Exception ex) { viewPlanZajec.Rezerwacje = new List<Rezerwacja>(); }

            viewPlanZajec.PodzielRezerwacje(uni);
            viewPlanZajec.Uzupelanieniedanych(uni);
            viewPlanZajec.SprawdzDniWolne(uni);

            var test = viewPlanZajec.Rezerwacje;

            return View(viewPlanZajec);
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

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Podane dane do logowania są niepoprawne!");
            }
            
            return View();
        }

        public ActionResult Wyloguj()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
