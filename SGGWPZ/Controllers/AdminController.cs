using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public string InnerMessage { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        [BindProperty]
        public dynamic Obiekt { get; set; }

        public AdminController(IUniversalRepositoryTypeOf UNI, PlanContext _DB)
        {
            uni = UNI;
            Db = _DB;
        }

        public IActionResult Index(string co)
        {
            if (HttpContext.Session.GetString("rodzaj_konta") == "Admin" || HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka" && co == "Rezerwacja")
            {
                Obiekt = uni.Obiekt(co);

                List<string> ListaItemowDoDodania = new List<string>();
                foreach (var item in Obiekt.GetType().GetProperties())
                    ListaItemowDoDodania.Add(item.Name);

                IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(co));

                List<dynamic> lista2 = lista.ToList();
                if (HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka") // Jezeli zalogowala sie sekretarka to wybieramy tylko jej rezerwacje
                { lista2 = lista2.FindAll(r => r.GetType().GetProperty("uzytkownikId").GetValue(r) == Convert.ToInt32(HttpContext.Session.GetString("uzytkownikId"))); }
                
                return View("Index", new ViewLista(ListaItemowDoDodania, lista2, co));
            }
            else
            {
                return RedirectToAction("Zaloguj", "Home", "");
            }
        }

        [HttpGet]
        public IActionResult Create(string co)
        {
            if (ModelState.IsValid &&
                HttpContext.Session.GetString("rodzaj_konta") == "Admin" || 
                HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka" 
                && co == "Rezerwacja")
            {
                ViewItem viewItem = new ViewItem();
                viewItem.Nazwa = co;

                try
                {
                    Obiekt = uni.Obiekt(co);
                    List<string> wartosci = new List<string>();
                    List<string> ListaItemowDoDodania = new List<string>(); // Konstruktor
                    List<Dictionary<string,string>> ListaListNazw = new List<Dictionary<string, string>>();
                    
                    foreach (var item in Obiekt.GetType().GetProperties())
                    {
                        // Jezeli obiekt ma klucze obce
                        if (item.Name != Obiekt.GetType().GetProperties()[0].Name && item.Name.Contains("Id"))
                        {
                            string nazwa = "";

                            foreach (var item2 in Db.GetType().GetProperties())
                            {
                                //if (item.Name.ToLower().Substring(0, 3) == item2.Name.ToLower().Substring(0, 3))
                                //{ nazwa = item2.Name; }
                                if (item2.Name.ToLower().Replace("_", "").Contains(item.Name.Split("I")[0].ToLower()))
                                { nazwa = item2.Name; }
                            }

                            IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(nazwa));
                            Dictionary<string, string> listanazw = new Dictionary<string, string>();
                            if (HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka" && nazwa == "Uzytkownik")
                            {
                                //List<dynamic> lista2 = lista.ToList();
                                var kto = lista.ToList().FirstOrDefault(u => u.login == HttpContext.Session.GetString("login"));
                                string key = Convert.ToString(kto.GetType().GetProperty("login").GetValue(kto));
                                listanazw[key] = Convert.ToString(kto.GetType().GetProperty("uzytkownikId").GetValue(kto));
                            }
                            else
                            {
                                foreach (var item2 in lista)
                                {
                                    string key = item2.GetType().GetProperties()[1].GetValue(item2);
                                    listanazw[key] = Convert.ToString(item2.GetType().GetProperties()[0].GetValue(item2));
                                }
                            }                                                 
                            ListaListNazw.Add(listanazw);
                        }
                        ListaItemowDoDodania.Add(item.Name);
                        wartosci.Add("");
                    }

                    // Dodaj pozostale dane do modelu widoku
                    viewItem.Naglowki = ListaItemowDoDodania;
                    viewItem.Wartosci = wartosci;
                    viewItem.ListaNazwKluczyObcych = ListaListNazw;                    

                    return View("Create", viewItem);
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    if (ex.InnerException != null)
                    InnerMessage = ex.InnerException.Message.ToString();
                    return View("Create",viewItem.Nazwa);
                }
            }
            else
            {
                return RedirectToAction("Zaloguj", "Home", "");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ViewItem item)
        {
            if (ModelState.IsValid)
            {                
                try
                {
                    //Jezeli nie ma wyboru klucza obcego
                    if (item.Wartosci.Count != item.Naglowki.Count)
                    { throw new Exception("Nie mozna dobrac klucza obcego - prawdopodobnie jest pusty. \nNajpierw stworz obiekt klucza obcego"); }

                    Obiekt = uni.Obiekt(item.Nazwa);

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

                    //Jakas informacja nie zostala wprowadzona
                    if (item.Wartosci.FindAll(it => it == null).Count > 1)
                    { throw new Exception("Prosze wypelnic wszystkie pola"); }

                    // Podany obiekt istnieje juz w bazie danych
                    if (uni.SprawdzCzyIstniejeWBazie(Obiekt) != null)
                    { throw new Exception("Taki obiekt istnieje juz w bazie"); }

                    await uni.CreateTAsync(Obiekt);

                    IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(item.Nazwa));
                    List<dynamic> lista2 = lista.ToList();
                    if (HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka") // Jezeli zalogowala sie sekretarka to wybieramy tylko jej rezerwacje
                    { lista2 = lista2.FindAll(r => r.GetType().GetProperty("uzytkownikId").GetValue(r) == Convert.ToInt32(HttpContext.Session.GetString("uzytkownikId"))); }

                    return View("Index", new ViewLista(item.Naglowki, lista2, item.Nazwa));
                }
                catch (Exception ex)
                {
                    item.ErrorMessage = ex.Message.ToString();
                    item.Wartosci = item.Naglowki.DefaultIfEmpty().ToList(); // Nie wiem jak ale dziala xd
                    item.ListaNazwKluczyObcych = GetListOfDictFK(uni.Obiekt(item.Nazwa));
                    if (ex.InnerException != null) { item.ErrorMessage += "\n" + ex.InnerException.Message.ToString(); };
                    return View("Create", item);
                }
            }

            return View("Lista");
        }

        [HttpGet]
        public IActionResult Update(int id, string co)
        {
            if (ModelState.IsValid &&
                HttpContext.Session.GetString("rodzaj_konta") == "Admin" ||
                HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka"
                && co == "Rezerwacja")
            {
                try
                {
                    Obiekt = uni.Obiekt(co);
                    //var klucze = uni.PartsOfPrimaryKey(Obiekt);
                    var Item = uni.ReadTAsync(id, Obiekt).Result;
                    List<string> wartosci = new List<string>();
                    //ListaKluczyObcych = new List<string>();
                    List<string> Naglowki = new List<string>(); // Konstruktor
                    List<Dictionary<string, string>> ListaListNazw = new List<Dictionary<string, string>>();

                    //IEnumerable<IForeignKey> foreignKeys = Db.Model.FindEntityType($"SGGWPZ.Models.{co}").GetForeignKeys();
                    //foreach (var foreignKey in foreignKeys)
                    //    ListaKluczyObcych.Add(foreignKey.PrincipalKey.ToString().Split(' ')[1].Split("PK")[0]);

                    List<string> ListaKluczy = uni.PartsOfAlternativeKey(Obiekt);

                    foreach (var item in Item.GetType().GetProperties())
                    {
                        // Jezeli obiekt ma klucze obce
                        if (item.Name != Obiekt.GetType().GetProperties()[0].Name && item.Name.Contains("Id"))
                        {
                            string nazwa = "";

                            foreach (var item2 in Db.GetType().GetProperties())
                            {
                                //if (item.Name.ToLower().Substring(0, 3) == item2.Name.ToLower().Substring(0, 3))
                                //{ nazwa = item2.Name; }
                                if (item2.Name.ToLower().Replace("_", "").Contains(item.Name.Split("I")[0].ToLower()))
                                { nazwa = item2.Name; }
                            }

                            IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(nazwa));
                            Dictionary<string, string> listanazw = new Dictionary<string, string>();
                            if (HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka" && nazwa == "Uzytkownik")
                            {
                                //List<dynamic> lista2 = lista.ToList();
                                var kto = lista.ToList().FirstOrDefault(u => u.login == HttpContext.Session.GetString("login"));
                                string key = Convert.ToString(kto.GetType().GetProperty("login").GetValue(kto));
                                listanazw[key] = Convert.ToString(kto.GetType().GetProperty("uzytkownikId").GetValue(kto));
                            }
                            else
                            {
                                foreach (var item2 in lista)
                                {
                                    string key = item2.GetType().GetProperties()[1].GetValue(item2);
                                    listanazw[key] = Convert.ToString(item2.GetType().GetProperties()[0].GetValue(item2));
                                }
                            }
                            ListaListNazw.Add(listanazw);
                        }

                        Naglowki.Add(item.Name);                   
                        wartosci.Add(Convert.ToString(Item.GetType().GetProperty(item.Name).GetValue(Item)));
                    }

                    ViewItem viewItem = new ViewItem();
                    viewItem.Nazwa = co;
                    viewItem.Naglowki = Naglowki;
                    viewItem.Wartosci = wartosci;
                    viewItem.ListaNazwKluczyObcych = ListaListNazw;

                    return View("Update", viewItem);
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    if (ex.InnerException != null) InnerMessage = ex.InnerException.Message.ToString();
                    return View();

                    //item.ErrorMessage = ex.Message.ToString();
                    //item.Wartosci = item.Naglowki.DefaultIfEmpty().ToList(); // Nie wiem jak ale dziala xd
                    //item.ListaNazwKluczyObcych = GetListOfDictFK(uni.Obiekt(item.Nazwa));
                    //if (ex.InnerException != null) { item.ErrorMessage += "\n" + ex.InnerException.Message.ToString(); };
                    //return View("Update", item);
                }
            }
            else
            {
                return RedirectToAction("Zaloguj", "Home", "");
            }
            //return View("Lista", co);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ViewItem item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Message = "Obiekt zostal zedytowany";
                    var Obiekt = uni.Obiekt(item.Nazwa);
                    int Id = Convert.ToInt32(item.Wartosci[0]);
                    //ListaKluczy = uni.PartsOfPrimaryKey(Obiekt);

                    for (int a = 0; a < Obiekt.GetType().GetProperties().Length; a++)
                    {
                        try
                        {
                            Obiekt.GetType().GetProperty(Obiekt.GetType().GetProperties()[a].Name)
                            .SetValue(Obiekt, item.Wartosci[a]);
                        }
                        catch (Exception)
                        {
                            Obiekt.GetType().GetProperty(Obiekt.GetType().GetProperties()[a].Name)
                            .SetValue(Obiekt, Convert.ToInt32(item.Wartosci[a]));
                        }
                    }

                    //Jakas informacja nie zostala wprowadzona
                    if (item.Wartosci.FindAll(it => it == null).Count > 1)
                    { throw new Exception("Prosze wypelnic wszystkie pola"); }

                    // Podany obiekt istnieje juz w bazie danych
                    if (uni.SprawdzCzyIstniejeWBazie(Obiekt) != null)
                    { throw new Exception("Taki obiekt istnieje juz w bazie"); }

                    await uni.UpdateTAsync(Obiekt);

                    IEnumerable<dynamic> lista = uni.ReadAllT(uni.Obiekt(item.Nazwa));
                    List<dynamic> lista2 = lista.ToList();
                    if (HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka") // Jezeli zalogowala sie sekretarka to wybieramy tylko jej rezerwacje
                    { lista2 = lista2.FindAll(r => r.GetType().GetProperty("uzytkownikId").GetValue(r) == Convert.ToInt32(HttpContext.Session.GetString("uzytkownikId"))); }

                    return View("Index", new ViewLista(item.Naglowki, lista2, item.Nazwa));
                }
                catch (Exception ex)
                {
                    item.ErrorMessage = ex.Message.ToString();
                    item.Wartosci = item.Naglowki.DefaultIfEmpty().ToList(); // Nie wiem jak ale dziala xd
                    item.ListaNazwKluczyObcych = GetListOfDictFK(uni.Obiekt(item.Nazwa));
                    if (ex.InnerException != null) { item.ErrorMessage += "\n" + ex.InnerException.Message.ToString(); };
                    return View("Update", item);
                }
            }

            return View();
        }

        public async Task<IActionResult> Delete(int Id, string co)
        {
            if (HttpContext.Session.GetString("rodzaj_konta") == "Admin" ||
                HttpContext.Session.GetString("rodzaj_konta") == "Sekretarka" && co == "Rezerwacje")
            {
                var Obiekt = uni.Obiekt(co);
                List<string> naglowki = uni.ListOfProperties(Obiekt);

                try
                {
                    // Sprawdz czy inne tabele nie korzystaja z danego wiersza (jako klucza obcego)
                    foreach (var tabela in Db.GetType().GetProperties().SkipLast(3)) // Dla kazdej tabeli w bazie
                    { if (tabela.Name.ToLower().Substring(0, 3) != co.ToLower().Substring(0, 3)) // Tylko nie dla siebie samej
                        { var objTabel = uni.Obiekt(tabela.Name);
                            foreach (var kolTabel in objTabel.GetType().GetProperties()) // Dla kazdej kolumny w tabeli
                            { if (kolTabel.Name.ToLower().Substring(0, 3) == co.ToLower().Substring(0, 3)) // Sprawdz czy zawiera kolumne ktora jest kluczem obcym dla tabeli
                                { foreach (var rowTabel in uni.ReadAllT(objTabel)) // Jezeli tak - Sprawdz dla kazdego wiersza
                                    { if (rowTabel.GetType().GetProperty(kolTabel.Name).GetValue(rowTabel) == Id) // Czy wiersz zawiera usuwany element jako klucz obcy
                                        { throw new Exception(String.Format("Najpierw usun wszystkie wiersze w innych tabelach zawierajace odniesienie do tego rekordu. Na przyklad w  tabeli  {0}", tabela.Name)); }
                                    }
                                } // Jezeli tak to ERROR
                            }
                        }
                    }

                    await uni.DeleteTAsync(Id, Obiekt);
                    return View("Index", new ViewLista(naglowki, uni.ReadAllT(Obiekt), co));
                }
                catch (Exception ex)
                { return View("Index", new ViewLista(naglowki, uni.ReadAllT(Obiekt), co, ex.Message)); }
            }
            else { return RedirectToAction("Zaloguj", "Home", ""); }            
        }
        
        // Metody pomocnicze

        public List<Dictionary<string,string>> GetListOfDictFK<T>(T Obiekt) where T: class
        {
            List<Dictionary<string, string>> ListaListNazw = new List<Dictionary<string, string>>();

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
                    Dictionary<string, string> listanazw = new Dictionary<string, string>();
                    foreach (var item2 in lista)
                    {
                        string key = item2.GetType().GetProperties()[1].GetValue(item2);
                        listanazw[key] = Convert.ToString(item2.GetType().GetProperties()[0].GetValue(item2));
                    }
                    ListaListNazw.Add(listanazw);
                }
            }

            return ListaListNazw;
        }

        
    }
}