using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGGWPZ.Models
{
    [Table("Rodzaj_uzytkownika")]
    public class Rodzaj_uzytkownika
    {
        [Key]
        public int rodzajuzytkownikaId { get; set; }
        public string rodzajuzytkownika { get; set; }
    }
}
