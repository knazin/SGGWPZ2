using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Kierunek")]
    public class Kierunek
    {
        [Key]
        public int kierunekId { get; set; }
        public string nazwa { get; set; }
    }
}
