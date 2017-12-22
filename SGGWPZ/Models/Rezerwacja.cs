using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SGGWPZ.Models
{
    [Table("Rezerwacja")]
    public partial class Rezerwacja
    {
        [Key]
        public int rezerwacjaId { get; set; }
        public string data { get; set; }
        public int przedmiotId { get; set; }
        public int salaId { get; set; }
        public int cyklicznoscId { get; set; }

        // Kto/Ktora sekretarka/admin stworzyla rezerwacje
        public int uzytkownikId { get; set; }

    }
}

    
