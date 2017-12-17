using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SGGWPZ.Models
{
    public class PlanContext : DbContext
    {
        public PlanContext(DbContextOptions<PlanContext> options) : base(options)
        {
        }

        public PlanContext()
        {

        }

        public DbSet<Cyklicznosci> Cyklicznosci { get; set; }
        public DbSet<Grupy> Grupy { get; set ;}
        public DbSet<Katedry> Katedry { get; set; }
        public DbSet<Przedmioty> Przedmioty { get; set; }
        public DbSet<Rezerwacje> Rezerwacje { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<Typy_przedmiotu> Typy_przedmiotu { get; set; }
        public DbSet<Wydzialy> Wydzialy { get; set; }
        public DbSet<Wykladowcy> Wykladowcy { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=C:\\Users\\Kac\\source\\repos\\SGGWPZ2\\planzajec2.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Katedry>().HasKey(k => k.katedraId);
            //modelBuilder.Entity<Katedry>().HasOne(k => k.wydzialyId) ;
                //.HasForeignKey()
                //.HasConstraintName("ForeignKey_Post_Blog");
        }

    }
}
