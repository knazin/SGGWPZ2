using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Rodzaj_przedmiotu")]
    public class Pz_Rodzaj_przedmiotu
    {
        [Key]
        public int rodzajprzedmiotuId { get; set; }
        public string rodzajprzedmiotu { get; set; } // Nieobowiazkowy, Obowiazkowy, Specjalizacyjny, Fakltet
    }
}
