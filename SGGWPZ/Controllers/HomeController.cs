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
            try
            {
                ViewPlanZajec vPZ = new ViewPlanZajec
                {
                    kierunek = "Informatyka",
                    semestr_studiow = "1",
                    grupa = "1",
                    datetime = DateTime.Now
                };

                try { vPZ.ZnajdzRezerwacje(uni); }
                catch (Exception) { }

                vPZ.PodzielRezerwacje(uni);
                vPZ.Uzupelanieniedanych(uni);
                vPZ.SprawdzDniWolne(uni);

                var test = vPZ.Rezerwacje;
                return View(vPZ);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message.ToString();
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Index(ViewPlanZajec viewPlanZajec, int ile, string dt)
        {
            // Przesuniecie daty
            if (dt != null){ viewPlanZajec.datetime = Convert.ToDateTime(dt).AddDays(ile); }
            else           { viewPlanZajec.datetime = viewPlanZajec.datetime.AddDays(ile); }
            viewPlanZajec.datetimestring = viewPlanZajec.datetime.ToString();

            try { viewPlanZajec.ZnajdzRezerwacje(uni); }
            catch (Exception) { viewPlanZajec.Rezerwacje = new List<Rezerwacja>(); }

            viewPlanZajec.PodzielRezerwacje(uni);
            viewPlanZajec.Uzupelanieniedanych(uni);
            viewPlanZajec.SprawdzDniWolne(uni);

            var test = viewPlanZajec.Rezerwacje;

            return View(viewPlanZajec);
        }

        // Wykladowcy

        public IActionResult Wykladowcy()
        {
            ViewPlanZajec vPZ = new ViewPlanZajec
            {
                wykladowcy = uni.ReadAllT(new Wykladowca()),
                datetime = DateTime.Now
            };
            vPZ.wykladowca = vPZ.wykladowcy[0].skrot_wykladowca;

            try { vPZ.ZnajdzRezerwacje(uni); }
            catch (Exception) { }

            vPZ.PodzielRezerwacje(uni);
            vPZ.Uzupelanieniedanych(uni);
            vPZ.SprawdzDniWolne(uni);

            var test = vPZ.Rezerwacje;
            return View("PlanWykladowcy",vPZ);
        }

        [HttpPost]
        public IActionResult Wykladowcy(ViewPlanZajec viewWykladowcy, int ile, string dt)
        {
            viewWykladowcy.wykladowcy = uni.ReadAllT(new Wykladowca());
            // Przesuniecie daty
            if (dt != null) { viewWykladowcy.datetime = Convert.ToDateTime(dt).AddDays(ile); }
            else { viewWykladowcy.datetime = viewWykladowcy.datetime.AddDays(ile); }
            viewWykladowcy.datetimestring = viewWykladowcy.datetime.ToString();

            try { viewWykladowcy.ZnajdzRezerwacje(uni); }
            catch (Exception) { viewWykladowcy.Rezerwacje = new List<Rezerwacja>(); }

            viewWykladowcy.PodzielRezerwacje(uni);
            viewWykladowcy.Uzupelanieniedanych(uni);
            viewWykladowcy.SprawdzDniWolne(uni);

            var test = viewWykladowcy.Rezerwacje;

            return View("PlanWykladowcy", viewWykladowcy);
        }

        // Sale

        public IActionResult Sale()
        {
            ViewPlanZajec vPZ = new ViewPlanZajec
            {
                sale = uni.ReadAllT(new Sala()),
                datetime = DateTime.Now
            };
            vPZ.sala = vPZ.sale[0].skrot_informacji;

            try { vPZ.ZnajdzRezerwacje(uni); }
            catch (Exception) { }

            vPZ.PodzielRezerwacje(uni);
            vPZ.Uzupelanieniedanych(uni);
            vPZ.SprawdzDniWolne(uni);

            return View("PlanSal", vPZ);
        }

        [HttpPost]
        public IActionResult Sale(ViewPlanZajec viewSale, int ile, string dt)
        {
            viewSale.sale = uni.ReadAllT(new Sala());
            // Przesuniecie daty
            if (dt != null) { viewSale.datetime = Convert.ToDateTime(dt).AddDays(ile); }
            else { viewSale.datetime = viewSale.datetime.AddDays(ile); }
            viewSale.datetimestring = viewSale.datetime.ToString();

            try { viewSale.ZnajdzRezerwacje(uni); }
            catch (Exception) { viewSale.Rezerwacje = new List<Rezerwacja>(); }

            viewSale.PodzielRezerwacje(uni);
            viewSale.Uzupelanieniedanych(uni);
            viewSale.SprawdzDniWolne(uni);

            return View("PlanSal", viewSale);
        }

        // Inne

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

        // Logowanie

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
