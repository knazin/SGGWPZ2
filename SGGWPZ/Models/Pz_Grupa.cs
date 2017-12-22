using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Grupa")]
    public partial class Pz_Grupa
    {
        [Key]
        public int grupaId { get; set; }
        public string grupy { get; set; } // Max 9 -> zapis jako liczba, np.: 168 (grupa 1 6 i 8)
        public int rokstudiowId { get; set; }
        public int kierunekId { get; set; }

        //public int wydzialId { get; set; }
        //public string rodzajstudiow { get; set; }
        //public string stopienstudiow { get; set; }
        //public string semestr { get; set; }
        //public int semestrstudiow { get; set; }
        //public int rokstudiow { get; set; }
        //public string rodzajprzedmiotu { get; set; }
        //public string rodzajzajec { get; set; }
    }
}

