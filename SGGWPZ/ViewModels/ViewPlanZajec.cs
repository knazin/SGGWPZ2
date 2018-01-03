using Newtonsoft.Json;
using SGGWPZ.Models;
using SGGWPZ.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SGGWPZ.ViewModels
{
    public class ViewPlanZajec
    {
        public string kierunek { get; set; }
        public string semestr_studiow { get; set; }
        public string grupa { get; set; }
        public List<string> DataTygodnia { get; set; }
        public List<Rezerwacja> Rezerwacje { get; set; } // Lista rezerwacji z rystrykcjami
        public List<List<Rezerwacja>> RezerwacjeDnia { get; set; } // Lista (pn, wt, ...) Listy rezerwacji z restrykcjami na dany dzien
        public List<List<Dictionary<string,string>>> RezerwacjeDniaDane { get; set; }

        public void ZnajdzRezerwacje(IUniversalRepositoryTypeOf uni)
        {
            var obiekt = uni.Obiekt("Rezerwacja");
           
            Rezerwacje = new List<Rezerwacja>();
            List<Rezerwacja> Rezerwacje2 = uni.ReadAllT(new Rezerwacja());
            List<Przedmiot> listaPrzedmiotow = uni.ReadAllT(new Przedmiot());

            // Znajdz przedmioty ktore maja ten sam kierunek i semestr studiow
            int idkierunku = uni.ReadAllT(new Kierunek()).FirstOrDefault(k => k.nazwa == kierunek).kierunekId;
            int idgrupy = uni.ReadAllT(new Grupa()).FirstOrDefault(g => g.grupy == grupa).grupaId;

            listaPrzedmiotow = listaPrzedmiotow.Where(p => p.kierunekId == idkierunku && p.semestr_studiow == semestr_studiow).ToList();

            Rezerwacje2 = Rezerwacje2.Where(r => r.grupaId == idgrupy).ToList();

            foreach (var przedmiot in listaPrzedmiotow)
            { Rezerwacje.AddRange(Rezerwacje2.Where(r => r.przedmiotId == przedmiot.przedmiotId)); }
        }

        public void PodzielRezerwacje(IUniversalRepositoryTypeOf uni)
        {
            //var asd = CultureInfo.InvariantCulture.Calendar;
            //DateTimeFormatInfo fmt = (new CultureInfo("hr-HR")).DateTimeFormat;

            DataTygodnia = new List<string>();
            List<List<Cyklicznosc>> listaCyklId = new List<List<Cyklicznosc>>();

            // Pobierz daty aktualnego miesiaca
            CultureInfo culture = new CultureInfo("pt-BR"); // dzien/miesiac/rok
            for (DayOfWeek i = DayOfWeek.Monday; i <= DayOfWeek.Saturday+1; i++)
            { DataTygodnia.Add(DateTime.Now.AddDays(i - DateTime.Now.DayOfWeek).Date.ToString("d",culture)); }

            // Wszystkie aktualne rezerwacje
            List<Cyklicznosc> aktualnecyklicznosci = uni.ReadAllT(new Cyklicznosc()).Where(c =>
               DateTime.Compare(Convert.ToDateTime(c.od_ktorego_dnia, culture), DateTime.Now) < 0 &&
               DateTime.Compare(Convert.ToDateTime(c.od_ktorego_dnia, culture), DateTime.Now) < 0)
               .ToList();

            for (DayOfWeek i = DayOfWeek.Monday; i <= DayOfWeek.Saturday; i++)
            { listaCyklId.Add(aktualnecyklicznosci.Where(r => Convert.ToDateTime(r.od_ktorego_dnia, culture).DayOfWeek == i).ToList()); }
            listaCyklId.Add(aktualnecyklicznosci.Where(r => Convert.ToDateTime(r.od_ktorego_dnia, culture).DayOfWeek == DayOfWeek.Sunday).ToList());

            // Uzupelnianie rezerwacji
            RezerwacjeDnia = new List<List<Rezerwacja>>();

            foreach (var listacykli in listaCyklId)
            {
                List<Rezerwacja> RezeDnia = new List<Rezerwacja>();
                //foreach (var cykl in listacykli)
                //{
                //    //var cykl = uni.ReadAllT(new Cyklicznosc()).First(c => c.cyklicznoscId == rezerwacja.cyklicznoscId);
                //    var rez = Rezerwacje.FirstOrDefault(r => r.cyklicznoscId == cykl.cyklicznoscId);
                //    RezeDnia.Add(rez);
                //}
                RezeDnia.AddRange(Rezerwacje.FindAll(r => listacykli.Any(c => r.cyklicznoscId == c.cyklicznoscId)));
                RezerwacjeDnia.Add(RezeDnia);              
            }
        }

        public void Uzupelanieniedanych(IUniversalRepositoryTypeOf uni)
        {
            RezerwacjeDniaDane = new List<List<Dictionary<string, string>>>();

            foreach (var rezerwacje in RezerwacjeDnia)
            {
                int i = 0;
                List<Dictionary<string, string>> DaneDnia = new List<Dictionary<string, string>>();
                foreach (var rezerwacja in rezerwacje)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();

                    var cykl = uni.ReadAllT(new Cyklicznosc()).First(c => c.cyklicznoscId == rezerwacja.cyklicznoscId);
                    var grup = uni.ReadAllT(new Grupa()).First(c => c.grupaId == rezerwacja.grupaId);
                    var prze = uni.ReadAllT(new Przedmiot()).First(c => c.przedmiotId == rezerwacja.przedmiotId);
                    var sala = uni.ReadAllT(new Sala()).First(c => c.salaId == rezerwacja.salaId);
                    var wykl = uni.ReadAllT(new Wykladowca()).First(c => c.wykladowcaId == prze.wykladowcaId);

                    foreach (var prop in prze.GetType().GetProperties())
                    { dictionary.Add(prop.Name,prze.GetType().GetProperty(prop.Name).GetValue(prze).ToString()); }

                    foreach (var prop in grup.GetType().GetProperties())
                    { dictionary.Add(prop.Name, grup.GetType().GetProperty(prop.Name).GetValue(grup).ToString()); }

                    foreach (var prop in cykl.GetType().GetProperties())
                    { dictionary.Add(prop.Name, cykl.GetType().GetProperty(prop.Name).GetValue(cykl).ToString()); }

                    foreach (var prop in sala.GetType().GetProperties())
                    { dictionary.Add(prop.Name, sala.GetType().GetProperty(prop.Name).GetValue(sala).ToString()); }

                    foreach (var prop in wykl.GetType().GetProperties())
                    { try { dictionary.Add(prop.Name, wykl.GetType().GetProperty(prop.Name).GetValue(wykl).ToString()); } catch (Exception) { } }

                    // Wysokosc pudla
                    var wys = Convert.ToString(Convert.ToInt32(dictionary["czas_trwania"]) * 2) + "px";
                    dictionary.Add("wysokosc", wys);

                    // Do godziny
                    var do_ktorej = Convert.ToDateTime(dictionary["od_ktorej_godziny"]).AddMinutes(Convert.ToInt32(dictionary["czas_trwania"])).ToShortTimeString().Split(" ")[0];
                    dictionary.Add("do_ktorej_godziny",do_ktorej);

                    // Kolor tla
                    dictionary.Add("tlo", "#548cbc");

                    // Kolor tekstu
                    dictionary.Add("tekst", "white");

                    string info = String.Format("<table style='border: solid 1px #e2e2e2'>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Nazwa przedmiotu</td><td style='padding: 3px'>{dictionary["nazwa"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Czas rozpoczecia:</td><td style='padding: 3px'>{dictionary["od_ktorej_godziny"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Czas trwania:</td><td style='padding: 3px'>{dictionary["czas_trwania"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Co ile tygodni:</td><td style='padding: 3px'>{dictionary["co_ile_tygodni"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Miejsce:</td><td style='padding: 3px'>{dictionary["skrot_informacji"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Rodzaj przedmiotu:</td><td style='padding: 3px'>{dictionary["rodzaj_przedmiotu"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Rodzaj zajec:</td><td style='padding: 3px'>{dictionary["rodzaj_zajec"]}</td></tr>" +
                        $"<tr style='border: solid 1px #e2e2e2'><td style='padding: 3px'>Wykladowca:</td><td style='padding: 3px'>{dictionary["imie"]} {dictionary["nazwisko"]}</td></tr>" +
                        "</table>");
                    dictionary.Add("info", info);

                    DaneDnia.Add(dictionary);

                    // Dodanie Przerwy
                    if (rezerwacja != rezerwacje.Last())
                    {
                        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();

                        var cykl2 = uni.ReadAllT(new Cyklicznosc()).First(c => c.cyklicznoscId == rezerwacje[i+1].cyklicznoscId);
                        i++;

                        foreach (var prop in cykl2.GetType().GetProperties())
                        { dictionary2.Add(prop.Name, cykl2.GetType().GetProperty(prop.Name).GetValue(cykl2).ToString()); }


                        // Nazwa bloku
                        dictionary2.Add("nazwa", "Przerwa");
                        
                        // Wysokosc
                        int ile_przerwy = (Convert.ToDateTime(dictionary2["od_ktorej_godziny"]) - Convert.ToDateTime(dictionary["do_ktorej_godziny"])).Minutes;
                        var wys2 = Convert.ToString(ile_przerwy * 2) + "px";
                        dictionary2.Add("wysokosc", wys2);

                        // Od godziny
                        dictionary2["od_ktorej_godziny"] = dictionary["do_ktorej_godziny"];

                        // Do godziny
                        var do_ktorej2 = Convert.ToDateTime(dictionary2["od_ktorej_godziny"]).AddMinutes(ile_przerwy).ToShortTimeString().Split(" ")[0];
                        dictionary2.Add("do_ktorej_godziny", do_ktorej2);

                        // Info
                        dictionary2.Add("info", "Pzerwa");

                        // Kolor tesktu
                        dictionary2.Add("tekst", "#548cbc");

                        // Kolor tla
                        dictionary2.Add("tlo", "white");

                        // Rodzaj zajec - aby klucz nie byl pusty
                        dictionary2.Add("rodzaj_zajec", "");

                        DaneDnia.Add(dictionary2);
                    }

                }
                RezerwacjeDniaDane.Add(DaneDnia);
            }

            //string json = JsonConvert.SerializeObject(RezerwacjeDniaDane[6][0]);
            //string json2 = JsonConvert.SerializeObject(RezerwacjeDniaDane[6]);
            //string json3 = JsonConvert.SerializeObject(RezerwacjeDniaDane);
        }
    }    
}
