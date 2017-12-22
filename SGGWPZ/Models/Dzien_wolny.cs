using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SGGWPZ.Models
{
    [Table("Dzien_wolny")]
    public class Dzien_wolny
    {
        public int dzienwolnyId { get; set; }
        public string powod_wolnego { get; set; }
        public string od_ktorego_dnia { get; set; }
        public string do_ktorego_dnia { get; set; }

    }
}
