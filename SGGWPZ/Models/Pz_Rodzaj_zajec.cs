using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Rodzaj_zajec")]
    public partial class Pz_Rodzaj_zajec
    {
        [Key]
        public int rodzajzajecId { get; set; }
        public string rodzajzajec { get; set; } // wykład/laboratoria/ćwiczenia/seminarium/seminarium wydziałowe/seminarium katedralne/egzamin/zaliczenie/inne
    }
}


    
