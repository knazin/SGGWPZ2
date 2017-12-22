using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SGGWPZ.Models
{
    [Table("Sala")]
    public partial class Sala
    {
        [Key]
        public int salaId { get; set; }
        public string nr_sali { get; set; }
        public string budynek { get; set; }
    }
}

    
