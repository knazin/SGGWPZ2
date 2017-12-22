using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGGWPZ.ViewModels
{
    public class ViewLista
    {
        // Cos tam
        public string Nazwa;
        public string ErrorMessage;
        //public dynamic Obiekt;
        public List<string> Naglowki;
        public List<string> Wartosci;
        public IEnumerable<dynamic> Obiekty;

        public ViewLista(List<string> naglowki, List<string> wartosci, IEnumerable<dynamic> obiekty, string nazwa)
        {
            Nazwa = nazwa;
            Wartosci = wartosci;
            Naglowki = naglowki;
            Obiekty = obiekty;
        }

        public ViewLista(List<string> naglowki, IEnumerable<dynamic> obiekty, string nazwa)
        {
            Nazwa = nazwa;
            Naglowki = naglowki;
            Obiekty = obiekty;
        }

        public ViewLista(List<string> naglowki, IEnumerable<dynamic> obiekty, string nazwa, string errormessage)
        {
            Nazwa = nazwa;
            Naglowki = naglowki;
            Obiekty = obiekty;
            ErrorMessage = errormessage;
        }
    }

    public class ViewItem
    {
        public string Nazwa { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Naglowki { get; set; }
        public List<string> Wartosci { get; set; }
        public List<Dictionary<string, string>> ListaNazwKluczyObcych { get; set; }

        //public ViewItem(List<string> naglowki, List<string> wartosci, string nazwa)
        //{
        //    Nazwa = nazwa;
        //    Wartosci = wartosci;
        //    Naglowki = naglowki;
        //}
    }
}
