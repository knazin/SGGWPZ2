using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SGGWPZ.Models
{
    [Table("Przedmiot")]
    public partial class Przedmiot
    {
        [Key]
        public int przedmiotId { get; set; }
        public string nazwa { get; set; }
        public string skrot { get; set; }
        public int wykladowcaId { get; set; }
        public int kierunekId { get; set; }
        public string rodzaj_studiow { get; set; }
        public string stopien_studiow { get; set; }
        public string semestr { get; set; }
        public string semestr_studiow { get; set; }
        public string rok_studiow { get; set; }
        public string rodzaj_przedmiotu { get; set; }
        public string rodzaj_zajec { get; set; }
        public int czas_trwania { get; set; } // minuty
        //public int czastrwaniaId { get; set; }
        //public int kierunekId { get; set; }
        //public int grupaId { get; set; }
        //public int rodzajprzedmiotuId { get; set; }
        //public int rodzajstudiowId { get; set; }
        //public int rodzajzajecId { get; set; }
        //public int rokstudiowId { get; set; }
        //public int semestrId { get; set; }
        //public int semestrstudiowId { get; set; }
        //public int stopienstudiowId { get; set; }

    }
}

    
