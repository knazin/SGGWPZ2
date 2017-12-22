using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Czas_trwania")]
    public class Pz_Czas_trwania
    {
        [Key]
        public int czastrwaniaId { get; set; }
        public string czastrwania { get; set; }
    }
}
