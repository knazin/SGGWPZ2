using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Cyklicznosc")]
    public class Cyklicznosc
    {
        [Key]
        public int cyklicznoscId { get; set; }
        public string skrot_cyklu { get; set; }
        public string od_ktorej_godziny { get; set; }
        public string od_ktorego_dnia { get; set; }
        public string do_ktorego_dnia { get; set; }
        public string co_ile_tygodni { get; set; }
        public string dzien_tygodnia { get; set; }
    }
}
    
