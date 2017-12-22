using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Katedra")]
    public partial class Katedra
    {
        [Key]
        public int katedraId { get; set; }
        public string nazwa { get; set; }
        public int kierunekId { get; set; }
    }
}

    
