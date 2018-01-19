using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Grupa")]
    public partial class Grupa
    {
        [Key]
        public int grupaId { get; set; }
        public string grupy { get; set; } // Max 9 -> zapis jako liczba, np.: 168 (grupa 1 6 i 8)

    }
}

