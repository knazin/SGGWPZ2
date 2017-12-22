using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Rok_studiow")]
    public class Pz_Rok_studiow
    {
        [Key]
        public int rokstudiowId { get; set; }
        public string rokstudiow { get; set; } // 1, 2, 3, 4, 5
    }
}
