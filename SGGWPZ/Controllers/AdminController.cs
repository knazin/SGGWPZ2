using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SGGWPZ.Repositories;
using SGGWPZ.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using SGGWPZ.ViewModels;

namespace SGGWPZ.Controllers
{
    public class AdminController : Controller
    {
        PlanContext Db;
        IUniversalRepositoryTypeOf uni;

        public AdminController(IUniversalRepositoryTypeOf UNI, PlanContext _DB)
        {
            uni = UNI;
            Db = _DB;
        }

        public IActionResult Index(string co)
        {
            Obiekt = uni.Obiekt(co);

            List<string> ListaItemowDoDodania = new List<string>();
            foreach (var item in Obiekt.GetType().GetProperties())
                ListaItemowDoDodania.Add(item.Name);

            IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(co));

            ViewLista viewLista = new ViewLista(ListaItemowDoDodania, lista, co);

            //Wykladowcy wykladowcy = new Wykladowcy();
            //wykladowcy.GetType().GetProperties()[0].SetValue(wykladowcy, 3);
            //var wyk = wykladowcy.GetType().GetProperty("wykladowcyId").GetValue(wykladowcy);

            return View("Index", viewLista);
        }

        [HttpGet]
        public IActionResult Create(string co)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Obiekt = uni.Obiekt(co);
                    List<string> wartosci = new List<string>();
                    //ListaKluczyObcych = new List<string>();
                    List<string> ListaItemowDoDodania = new List<string>(); // Konstruktor
                    List<IEnumerable<dynamic>> ListaListNazw = new List<IEnumerable<dynamic>>();

                    //IEnumerable<IForeignKey> foreignKeys = Db.Model.FindEntityType($"SGGWPZ.Models.{co}").GetForeignKeys();
                    //foreach (var foreignKey in foreignKeys)
                    //    ListaKluczyObcych.Add(foreignKey.PrincipalKey.ToString().Split(' ')[1].Split("PK")[0]);

                    //List<string> ListaKluczy = uni.PartsOfAlternativeKey(Obiekt);
                    
                    foreach (var item in Obiekt.GetType().GetProperties())
                    {
                        // Jezeli obiekt ma klucze obce
                        if (item.Name != Obiekt.GetType().GetProperties()[0].Name && item.Name.Contains("Id"))
                        {
                            string nazwa = "";

                            foreach (var item2 in Db.GetType().GetProperties())
                            {
                                if (item.Name.ToLower().Substring(0, 3) == item2.Name.ToLower().Substring(0, 3))
                                { nazwa = item2.Name; }
                            }

                            IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(nazwa));
                            List<dynamic> listanazw = new List<dynamic>();
                            foreach (var item2 in lista)
                            {
                                var test = item2.GetType().GetProperties()[1].Name;
                            }
                            ListaListNazw.Add(listanazw);
                        }
                        ListaItemowDoDodania.Add(item.Name);
                        wartosci.Add("");
                    }

                    ViewItem viewItem = new ViewItem();
                    viewItem.Nazwa = co;
                    viewItem.Naglowki = ListaItemowDoDodania;
                    viewItem.Wartosci = wartosci;

                    return View("Create", viewItem);
                }
                catch (Exception ex) { Message = ex.Message; if (ex.InnerException != null) InnerMessage = ex.InnerException.Message.ToString(); return View(); }
            }
            return View("Lista", co);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ViewItem item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Obiekt = uni.Obiekt(item.Nazwa);
                    //List<string> ListaItemowDoDodania = cos.Naglowki;

                    //ListaKluczy = uni.PartsOfPrimaryKey(uni.Obiekt(cos.Nazwa));

                    for (int a = 0; a < item.Naglowki.Count; a++)
                    {
                        try
                        {
                            Obiekt.GetType().GetProperty(item.Naglowki[a])
                            .SetValue(Obiekt, item.Wartosci[a]);
                        }
                        catch (Exception)
                        { // Jesli wartosc jest intem
                            Obiekt.GetType().GetProperty(item.Naglowki[a])
                            .SetValue(Obiekt, Convert.ToInt32(item.Wartosci[a]));
                        }
                    }
                    await uni.CreateTAsync(Obiekt);

                    return View("Index", new ViewLista(item.Naglowki, uni.ReadAllT(Obiekt), item.Nazwa));
                }
                catch (Exception ex)
                {
                    Message = ex.Message.ToString();
                    if (ex.InnerException != null) { InnerMessage = ex.InnerException.Message.ToString(); };
                }
            }

            return View("Lista");
        }

        [HttpGet]
        public IActionResult Update(int id, string co)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Obiekt = uni.Obiekt(co);
                    //var klucze = uni.PartsOfPrimaryKey(Obiekt);
                    var Item = uni.ReadTAsync(id, Obiekt).Result;
                    List<string> wartosci = new List<string>();
                    //ListaKluczyObcych = new List<string>();
                    List<string> Naglowki = new List<string>(); // Konstruktor

                    //IEnumerable<IForeignKey> foreignKeys = Db.Model.FindEntityType($"SGGWPZ.Models.{co}").GetForeignKeys();
                    //foreach (var foreignKey in foreignKeys)
                    //    ListaKluczyObcych.Add(foreignKey.PrincipalKey.ToString().Split(' ')[1].Split("PK")[0]);

                    List<string> ListaKluczy = uni.PartsOfAlternativeKey(Obiekt);

                    foreach (var item in Item.GetType().GetProperties())
                    {
                        Naglowki.Add(item.Name);                   
                        wartosci.Add(Convert.ToString(Item.GetType().GetProperty(item.Name).GetValue(Item)));
                    }

                    ViewItem viewItem = new ViewItem();
                    viewItem.Nazwa = co;
                    viewItem.Naglowki = Naglowki;
                    viewItem.Wartosci = wartosci;

                    return View("Update", viewItem);
                }
                catch (Exception ex) { Message = ex.Message; if (ex.InnerException != null) InnerMessage = ex.InnerException.Message.ToString(); return View(); }
            }
            return View("Lista", co);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ViewItem Item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Message = "Obiekt zostal zedytowany";
                    var Obiekt = uni.Obiekt(Item.Nazwa);
                    int Id = Convert.ToInt32(Item.Wartosci[0]);
                    //ListaKluczy = uni.PartsOfPrimaryKey(Obiekt);

                    for (int a = 0; a < Obiekt.GetType().GetProperties().Length; a++)
                    {
                        try
                        {
                            Obiekt.GetType().GetProperty(Obiekt.GetType().GetProperties()[a].Name)
                            .SetValue(Obiekt, Item.Wartosci[a]);
                        }
                        catch (Exception)
                        {
                            Obiekt.GetType().GetProperty(Obiekt.GetType().GetProperties()[a].Name)
                            .SetValue(Obiekt, Convert.ToInt32(Item.Wartosci[a]));
                        }
                    }

                    await uni.UpdateTAsync(Obiekt);

                    return View("Index", new ViewLista(Item.Naglowki, uni.ReadAllT(Obiekt), Item.Nazwa));
                }
                catch (Exception ex)
                {
                    //Message = ex.Message.ToString();
                    //if (ex.InnerException != null) { InnerMessage = ex.InnerException.Message.ToString(); };
                    //return View();
                }
            }

            return View();
        }

        public async Task<IActionResult> Delete(int Id, string co)
        {
            var Obiekt = uni.Obiekt(co);
            List<string> naglowki = uni.ListOfProperties(Obiekt);
            try
            {                
                await uni.DeleteTAsync(Id, Obiekt);
                return View("Index", new ViewLista(naglowki,uni.ReadAllT(Obiekt),co));
            }
            catch (Exception ex) { }//Message = $"Blad - Element nie zosta³ usuniety \n {ex.Message}"; return OnGet(NazTab); }


            return View("Index", new ViewLista(naglowki, uni.ReadAllT(Obiekt), co));
        }

        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public string InnerMessage { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        [BindProperty]
        public dynamic Obiekt { get; set; }

        //private string nazwaObiektu;
        //[BindProperty]
        //public string NazwaObiektu
        //{
        //    get { return SessionExt.Get<string>(HttpContext.Session, TabKeys.NazwaTabeli); }
        //}


        //[BindProperty]
        //public List<string> ListaKluczy { get; set; }

        //[BindProperty]
        //public List<string> ListaKluczyObcych { get; set; }

        //[BindProperty]
        //public List<string> ListaItemowDoDodania { get; set; }

        

        
    }
}