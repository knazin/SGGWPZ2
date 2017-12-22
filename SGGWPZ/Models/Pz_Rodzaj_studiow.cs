using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Rodzaj_studiow")]
    public class Pz_Rodzaj_studiow
    {
        [Key]
        public int rodzajstudiowId { get; set; }
        public string rodzajstudiow { get; set; } // Stacjonarne, Niestacjonarne
    }
}
