using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Semestr")]
    public class Pz_Semestr
    {
        [Key]
        public int semestrId { get; set; }
        public string semestr { get; set; } // zimowy, letni
    }
}
