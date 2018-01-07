using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SGGWPZ.Models
{
    [Table("Wolne")]
    public class Wolne
    {
        [Key]
        public int wolny_dzienId { get; set; }
        public string powod_wolnego { get; set; }
        public string wolne_od_ktorego_dnia { get; set; }
        public string wolne_do_ktorego_dnia { get; set; }

    }
}
