using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Semestr_studiow")]
    public class Pz_Semestr_studiow
    {
        [Key]
        public int semestrstudiowId { get; set; }
        public string semestrstudiow { get; set; } // 1,2,3,4,5,6,7,8,9,10
    }
}
