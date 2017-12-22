using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Pz_Stopien_studiow")]
    public class Pz_Stopien_studiow
    {
        [Key]
        public int stopienstudiowId { get; set; }
        public string stopienstudiow { get; set; } // 1, 2
    }
}
