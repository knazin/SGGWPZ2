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
        public string datetimestring { get; set; }
        public DateTime datetime { get; set; }
        public List<string> DataTygodnia { get; set; }
        public List<Rezerwacja> Rezerwacje { get; set; } // Lista rezerwacji z rystrykcjami
        public List<List<Rezerwacja>> RezerwacjeDnia { get; set; } // Lista (pn, wt, ...) Listy rezerwacji z restrykcjami na dany dzien
        public List<List<Dictionary<string,string>>> RezerwacjeDniaDane { get; set; }

        // Contructor 
        public ViewPlanZajec() { }
        // Constructor
        public ViewPlanZajec(string _kierunek, string _semestr_studiow, string _grupa)
        { kierunek = _kierunek;   semestr_studiow = _semestr_studiow;     grupa = _grupa; }

        public void ZnajdzRezerwacje(IUniversalRepositoryTypeOf uni)
        {
            var obiekt = uni.Obiekt("Rezerwacja");
           
            Rezerwacje = new List<Rezerwacja>();
            List<Rezerwacja> Rezerwacje2 = uni.ReadAllT(new Rezerwacja());
            List<Przedmiot> listaPrzedmiotow = uni.ReadAllT(new Przedmiot());

            // Znajdz przedmioty ktore maja ten sam kierunek i semestr studiow
            int idkierunku = uni.ReadAllT(new Kierunek()).FirstOrDefault(k => k.nazwa_kierunku == kierunek).kierunekId;

            //int idgrupy = uni.ReadAllT(new Grupa()).FirstOrDefault(g => g.grupy == grupa).grupaId; // Nowe na dole
            List<int> listaidgrupy = uni.ReadAllT(new Grupa()).Where(g => g.grupy.Contains(grupa)).ToList().Select(g => g.grupaId).ToList();

            listaPrzedmiotow = listaPrzedmiotow.Where(p => p.kierunekId == idkierunku && p.semestr_studiow == semestr_studiow).ToList();

            //Rezerwacje2 = Rezerwacje2.Where(r => r.grupaId == idgrupy).ToList(); // Nowe na dole
            List<Rezerwacja> Rezerwacje21 = Rezerwacje2.Where(r => listaidgrupy.Any(gid => gid == r.grupaId)).ToList();        

            foreach (var przedmiot in listaPrzedmiotow)
            { Rezerwacje.AddRange(Rezerwacje21.Where(r => r.przedmiotId == przedmiot.przedmiotId)); } //bylo Rezerwacje2
        }

        public void PodzielRezerwacje(IUniversalRepositoryTypeOf uni)
        {
            //var asd = CultureInfo.InvariantCulture.Calendar;
            //DateTimeFormatInfo fmt = (new CultureInfo("hr-HR")).DateTimeFormat;

            DataTygodnia = new List<string>();
            List<List<Cyklicznosc>> listaCyklId = new List<List<Cyklicznosc>>();

            // Pobierz daty tygodnia aktualnego miesiaca
            CultureInfo culture = new CultureInfo("pt-BR"); // dzien/miesiac/rok
            for (DayOfWeek i = DayOfWeek.Monday; i <= DayOfWeek.Saturday+1; i++)
            { DataTygodnia.Add(datetime.AddDays(i - datetime.DayOfWeek).Date.ToString("d",culture)); }
            

            // Wszystkie aktualne rezerwacje
            List<Cyklicznosc> aktualnecyklicznosci = uni.ReadAllT(new Cyklicznosc()).Where(c =>
               DateTime.Compare(Convert.ToDateTime(c.od_ktorego_dnia, culture).AddDays(-1), datetime) < 0 && // -1 bo tydzien zaczyna sie od Niedzieli (czyli pon musi porownac do poprzedniej niedzieli)
               DateTime.Compare(Convert.ToDateTime(c.do_ktorego_dnia, culture), datetime) > 0) // wczesniej DateTime.Compare(Convert.ToDateTime(c.od_ktorego_dnia, culture), DateTime.Now) < 0
               .ToList();
            
            // Wyrzucenie rezerwacji ktore sa np co 2 tygodnie a tydzien jest nieparzysty itp
            List<Cyklicznosc> aktualnecyklicznosc2 = new List<Cyklicznosc>();
            foreach (Cyklicznosc item in aktualnecyklicznosci)
            {
                if (item.co_ile_tygodni != "1")
                {
                    int coile = Convert.ToInt32(item.co_ile_tygodni);
                    var roznicatygodni = (Convert.ToDateTime(datetime, culture) - Convert.ToDateTime(item.od_ktorego_dnia, culture)).Days/7;
                    if(roznicatygodni % coile == 0)
                    { aktualnecyklicznosc2.Add(item); }
                }
                else { aktualnecyklicznosc2.Add(item); }
            }
            aktualnecyklicznosci = aktualnecyklicznosc2;

            //for (DayOfWeek i = DayOfWeek.Monday; i <= DayOfWeek.Saturday; i++)
            //{ listaCyklId.Add(aktualnecyklicznosci.Where(r => Convert.ToDateTime(r.od_ktorego_dnia, culture).DayOfWeek == i).ToList()); }
            //listaCyklId.Add(aktualnecyklicznosci.Where(r => Convert.ToDateTime(r.od_ktorego_dnia, culture).DayOfWeek == DayOfWeek.Sunday).ToList());

            for (DayOfWeek i = DayOfWeek.Monday; i <= DayOfWeek.Saturday; i++)
            { listaCyklId.Add(aktualnecyklicznosci.Where(r => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), r.dzien_tygodnia) == i).ToList()); }
            listaCyklId.Add(aktualnecyklicznosci.Where(r => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), r.dzien_tygodnia) == DayOfWeek.Sunday).ToList());

            // Uzupelnianie rezerwacji
            RezerwacjeDnia = new List<List<Rezerwacja>>();

            foreach (var listacykli in listaCyklId)
            {
                List<Rezerwacja> RezeDnia = new List<Rezerwacja>();

                List<Rezerwacja> list2 = new List<Rezerwacja>();
                foreach (var cykl in listacykli.OrderBy(c => Convert.ToDateTime(c.od_ktorej_godziny)).ToList())
                {
                    if(Rezerwacje.FirstOrDefault(r => r.cyklicznoscId == cykl.cyklicznoscId) != null)
                    { list2.Add(Rezerwacje.FirstOrDefault(r => r.cyklicznoscId == cykl.cyklicznoscId)); }                  
                }

                RezeDnia.AddRange(list2);
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

                    // Sortowanie Cyklicznosci w danym dniu ~ Sortowanie od najwczesniejszej do najpozniejszej godziny -> bo wtedy nie zle przerwy
                    var cykl = uni.ReadAllT(new Cyklicznosc()).First(c => c.cyklicznoscId == rezerwacja.cyklicznoscId);
                    var grup = uni.ReadAllT(new Grupa()).First(c => c.grupaId == rezerwacja.grupaId);
                    var prze = uni.ReadAllT(new Przedmiot()).First(c => c.przedmiotId == rezerwacja.przedmiotId);
                    var sala = uni.ReadAllT(new Sala()).First(c => c.salaId == rezerwacja.salaId);
                    var wykl = uni.ReadAllT(new Wykladowca()).First(c => c.wykladowcaId == prze.wykladowcaId);

                    //Jezeli 1 zajecia nie zaczynaja sie o 8 rano
                    if(cykl.od_ktorej_godziny != "8:00" && rezerwacje[0] == rezerwacja)
                    {
                        Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
                        Cyklicznosc cykl2 = new Cyklicznosc();
                        cykl2.od_ktorej_godziny = "8:00";
                        // Wysokosc
                        int ile_przerwy = (int)(Convert.ToDateTime(cykl.od_ktorej_godziny)-Convert.ToDateTime(cykl2.od_ktorej_godziny)).TotalMinutes;
                        var wys2 = Convert.ToString(ile_przerwy * 2) + "px";
                        dictionary3.Add("wysokosc", wys2);

                        // Nazwa bloku
                        dictionary3.Add("nazwa", "Przerwa");
                        // Od godziny
                        dictionary3["od_ktorej_godziny"] = "8:00";
                        // Do godziny
                        dictionary3.Add("do_ktorej_godziny", cykl.od_ktorej_godziny);
                        // Info
                        dictionary3.Add("info", "Przerwa");
                        // Kolor tesktu
                        dictionary3.Add("tekst", "#548cbc");
                        // Kolor tla
                        dictionary3.Add("tlo", "white");
                        // Rodzaj zajec - aby klucz nie byl pusty
                        dictionary3.Add("rodzaj_zajec", "");

                        DaneDnia.Add(dictionary3);
                    }

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
                        int ile_przerwy = (
                            (Convert.ToDateTime(dictionary2["od_ktorej_godziny"]) - Convert.ToDateTime(dictionary["do_ktorej_godziny"])).Minutes +
                            ((Convert.ToDateTime(dictionary2["od_ktorej_godziny"]) - Convert.ToDateTime(dictionary["do_ktorej_godziny"])).Hours*60)
                            );
                        var wys2 = Convert.ToString(ile_przerwy * 2) + "px";
                        dictionary2.Add("wysokosc", wys2);

                        // Od godziny
                        dictionary2["od_ktorej_godziny"] = dictionary["do_ktorej_godziny"];

                        // Do godziny
                        var do_ktorej2 = Convert.ToDateTime(dictionary2["od_ktorej_godziny"]).AddMinutes(ile_przerwy).ToShortTimeString().Split(" ")[0];
                        dictionary2.Add("do_ktorej_godziny", do_ktorej2);

                        // Info
                        dictionary2.Add("info", "Przerwa");

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
        }

        public void SprawdzDniWolne(IUniversalRepositoryTypeOf uni)
        {
            CultureInfo culture = new CultureInfo("pt-BR"); // dzien/miesiac/rok

            List<string> elementy = DataTygodnia.Last().Split("/").ToList();
            DateTime ldow = new DateTime(Convert.ToInt32(elementy[2]), Convert.ToInt32(elementy[1]), Convert.ToInt32(elementy[0]));
            List<string> elementy2 = DataTygodnia[0].Split("/").ToList();
            DateTime fdow = new DateTime(Convert.ToInt32(elementy2[2]), Convert.ToInt32(elementy2[1]), Convert.ToInt32(elementy2[0]));

            List<Wolne> dni_wolne = uni.ReadAllT(new Wolne()).Where(c =>
               DateTime.Compare(Convert.ToDateTime(c.wolne_od_ktorego_dnia, culture), ldow) <= 0 &&
               DateTime.Compare(Convert.ToDateTime(c.wolne_do_ktorego_dnia, culture), fdow) >= 0)
               .ToList();

            List<List<Dictionary<string, string>>> RezerwacjeDniaDane2 = new List<List<Dictionary<string, string>>>();

            Wolne dw = new Wolne();
            try { dw = dni_wolne[0]; }
            catch (Exception) { }
            
            int i = 0;
            foreach (var DataDnia in DataTygodnia)
            {
                List<string> list2 = DataDnia.Split("/").ToList();
                DateTime datadnia = new DateTime(Convert.ToInt32(list2[2]), Convert.ToInt32(list2[1]), Convert.ToInt32(list2[0]));
                if( Convert.ToDateTime(dw.wolne_od_ktorego_dnia, culture) <= datadnia && datadnia <= Convert.ToDateTime(dw.wolne_do_ktorego_dnia, culture))
                {
                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();

                    // Wysokosc pudla
                    dictionary.Add("wysokosc", "1680px");
                    // Nazwa bloku
                    dictionary.Add("nazwa", dw.powod_wolnego);
                    // Info
                    dictionary.Add("info", "Przerwa");
                    // Kolor tesktu
                    dictionary.Add("tekst", "#548cbc");
                    // Kolor tla
                    dictionary.Add("tlo", "white");
                    // Rodzaj zajec - aby klucz nie byl pusty
                    dictionary.Add("rodzaj_zajec", "");
                    // Od godziny
                    dictionary["od_ktorej_godziny"] = "8:00";
                    // Do godziny
                    dictionary.Add("do_ktorej_godziny", "21:45");

                    list.Add(dictionary);
                    RezerwacjeDniaDane2.Add(list);
                }
                else
                {
                    RezerwacjeDniaDane2.Add(RezerwacjeDniaDane[i]);
                }
                i++;
            }

            RezerwacjeDniaDane = RezerwacjeDniaDane2;

            string stop = "";



        }
    }    
}
