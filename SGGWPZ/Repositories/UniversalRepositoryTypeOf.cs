using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGGWPZ.Models;
using System.Globalization;
using SGGWPZ.ViewModels;

namespace SGGWPZ.Repositories
{
    public class UniversalRepositoryTypeOf : IUniversalRepositoryTypeOf
    {
        private PlanContext db;

        public UniversalRepositoryTypeOf(PlanContext planContext)
        {
            db = planContext;
        }

        public async Task<T> CreateTAsync<T>(T newT) where T : class
        {
            await db.Set<T>().AddAsync(newT);
            await db.SaveChangesAsync();
           
            return newT;
        }

        public async Task<bool> DeleteTAsync<T>(int ID, T Titem) where T : class
        {
            //throw new NotImplementedException();

            var nazwaId = PartsOfPrimaryKey(Titem)[0];
            var deletedT = await db.Set<T>().FirstOrDefaultAsync(a => (int)a.GetType().GetProperty(nazwaId).GetValue(a) == ID);

            if (deletedT != null)
            {

                db.Set<T>().Remove(deletedT);
                //db.SaveChanges();
                await db.SaveChangesAsync();
            }

            return (db.Set<T>().FirstOrDefault(a => (int)a.GetType().GetProperty(nazwaId).GetValue(a) == ID) == null);
        }

        public List<string> ListOfProperties<T>(T obiektT) where T : class
        {
            //throw new NotImplementedException();
            List<string> listWlasci = new List<string>();

            foreach (var item in obiektT.GetType().GetProperties())
            {
                listWlasci.Add(item.Name);
            }

            return listWlasci;
        }

        public dynamic Obiekt(string nazwaTabeli) => Activator.CreateInstance(Type.GetType($"SGGWPZ.Models.{nazwaTabeli}"));

        /// <summary>
        /// Pobierz klucze alternatywne obiektu
        /// </summary>
        /// <typeparam name="T1">Generyczny T</typeparam>
        /// <param name="obiekt"></param>
        /// <returns></returns>
        public List<string> PartsOfAlternativeKey<T1>(T1 obiekt) where T1 : class
        {
            //throw new NotImplementedException();

            List<string> listaKluczy = new List<string>();

            foreach (var item in db.Model.FindEntityType($"SGGWPZ.Models.{obiekt.GetType().Name}").FindPrimaryKey().Properties.ToList())
                listaKluczy.Add(item.Name);

            return listaKluczy;
        }

        public List<string> PartsOfPrimaryKey<T1>(T1 obiekt) where T1 : class
        {
            //throw new NotImplementedException();

            List<string> listaKluczy = new List<string>();

            foreach (var item in db.Model.FindEntityType($"SGGWPZ.Models.{obiekt.GetType().Name}").FindPrimaryKey().Properties.ToList())
                listaKluczy.Add(item.Name);

            return listaKluczy;
        }

        public List<dynamic> PobierzTablice(string nazwaTablicy)
        {
            throw new NotImplementedException();
        }

        public string PolecenieStworzeniaPierwszegoElementuWTablicy(string NazwaTabeli)
        {
            throw new NotImplementedException();
        }

        public List<T> ReadAllT<T>(T item) where T : class
        {
            //throw new NotImplementedException();
            return db.Set<T>().ToList();
        }

        public async Task<List<T>> ReadAllTAsync<T>(T item) where T : class
        {
            return await db.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> ReadTAsync<T>(int ID, T item) where T : class //bylo bez string nazwaId
        {
            var nazwaId = PartsOfPrimaryKey(item)[0];
            return await db.Set<T>().AsNoTracking().FirstOrDefaultAsync(i => (int)i.GetType().GetProperty(nazwaId).GetValue(i) == ID); // bylo Id
        }

        public List<T1> SortujPo<T1>(string PoCzymSortuj, T1 item) where T1 : class
        {
            try { return ReadAllT(item).OrderBy(i => i.GetType().GetProperty(PoCzymSortuj).GetValue(i)).ToList(); }
            catch (Exception) { return ReadAllT(item); }
        }

        public T SprawdzCzyIstniejeWBazie<T>(T obiekt) where T : class
        {
            List<string> listaKluczy = PartsOfPrimaryKey(obiekt);
            List<string> wlasciwosci = ListOfProperties(obiekt);
            wlasciwosci.Remove(listaKluczy[0]); // Usuwamy klucz glowny - bo kazdy nowy obiekt domyslnie ma id 0

            IEnumerable<dynamic> listaObiektowZBazy = ReadAllT(obiekt);
            bool czyZawieraTakieSameElementyKlucza;

            foreach (var item in db.Set<T>().ToList())
            {
                czyZawieraTakieSameElementyKlucza = wlasciwosci.All(x => item.GetType().GetProperty(x).GetValue(item).ToString() == obiekt.GetType().GetProperty(x).GetValue(obiekt).ToString());
                if (czyZawieraTakieSameElementyKlucza) { return item; }
            }

            return null; // brak obiektu
        }

        public async Task<T> UpdateTAsync<T>(T updatedT) where T : class
        {    
            var nazwaId = PartsOfPrimaryKey(updatedT)[0];
            var orginalT = await db.Set<T>().FirstOrDefaultAsync(p => (int)p.GetType().GetProperty(nazwaId).GetValue(p) == (int)updatedT.GetType().GetProperty(nazwaId).GetValue(updatedT));

            foreach (var item in updatedT.GetType().GetProperties()) //Przepisuje wszystkie Wlasciwosci
            {
                orginalT.GetType().GetProperty(item.Name).SetValue(
                    orginalT,
                    item.GetValue(updatedT)
                    );
            }

            //db.SaveChanges();
            await db.SaveChangesAsync();

            return orginalT;
        }

        public List<T1> WszystkieOpcje<T1>(string PoCzym, string Nazwa, string PoCzymSortuj, T1 Titem) where T1 : class
        {
            throw new NotImplementedException();
        }

        public List<T> WyszukajPoSymbolu<T>(string PoCzym, string Nazwa, T item) where T : class
        {
            //throw new NotImplementedException();

            if (PoCzym == null || PoCzym == "")
                return ReadAllT(item);
            else
            {
                List<T> akcj = ReadAllT(item);

                if (akcj.Where(a => (string)a.GetType().GetProperty(PoCzym).GetValue(a) == Nazwa).ToList().Count != 0)
                    return akcj.Where(a => (string)a.GetType().GetProperty(PoCzym).GetValue(a) == Nazwa).ToList();
                else
                    return ReadAllT(item);
            }
        }
    }
}

