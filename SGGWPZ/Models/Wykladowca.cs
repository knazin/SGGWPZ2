using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SGGWPZ.Models
{
    [Table("Wykladowca")]
    public partial class Wykladowca
    {
        [Key]
        public int wykladowcaId { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string tytul { get; set; }
        public int katedraId { get; set; }
        public int pesel { get; set; }
    }
}

    
