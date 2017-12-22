using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Uzytkownik")]
    public class Uzytkownik
    {
        [Key]
        public int uzytkownikId { get; set; }
        public string login { get; set; }
        public string haslo { get; set; }
        public int rodzajuzytkownikaId { get; set; }
    }
}
