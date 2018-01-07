﻿using System;
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

        public DbSet<Cyklicznosc> Cyklicznosc { get; set; }
        public DbSet<Katedra> Katedra { get; set; }
        public DbSet<Kierunek> Kierunek { get; set; }

        public DbSet<Przedmiot> Przedmiot { get; set; }
        public DbSet<Grupa> Grupa { get; set; }
        public DbSet<Wolne> Wolne { get; set; }
        //public DbSet<Pz_Czas_trwania> Pz_Czas_trwania { get; set; }
        //public DbSet<Grupa> Pz_Grupa { get; set; }
        //public DbSet<Pz_Rodzaj_przedmiotu> Pz_Rodzaj_przedmiotu { get; set; }
        //public DbSet<Pz_Rodzaj_studiow> Pz_Rodzaj_studiow { get; set; }
        //public DbSet<Pz_Rodzaj_zajec> Pz_Rodzaj_zajec { get; set; }
        //public DbSet<Pz_Rok_studiow> Pz_Rok_studiow { get; set; }
        //public DbSet<Pz_Semestr> Pz_Semestr { get; set; }
        //public DbSet<Pz_Semestr_studiow> Pz_Semestr_studiow { get; set; }
        //public DbSet<Pz_Stopien_studiow> Pz_Stopien_studiow { get; set; }

        public DbSet<Rezerwacja> Rezerwacja { get; set; }
        public DbSet<Rodzaj_uzytkownika> Rodzaj_uzytkownika { get; set; }
        public DbSet<Sala> Sala { get; set; }
        public DbSet<Uzytkownik> Uzytkownik { get; set; }
        public DbSet<Wykladowca> Wykladowca { get; set; }

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
