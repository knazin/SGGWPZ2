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

        /// <summary>
        /// Metoda dodajaca nowy rekord do bazy
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="newT">Obiekt reprezentujacy rekord bazodanowy</param>
        /// <returns>Nowo stworzony rekord</returns>
        public async Task<T> CreateTAsync<T>(T newT) where T : class
        {
            await db.Set<T>().AddAsync(newT);
            await db.SaveChangesAsync();
           
            return newT;
        }

        /// <summary>
        /// Metoda usuwajaca rekord z bazy
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="ID">Id rekordu do usuniecia</param>
        /// <param name="Titem">Obiekt do usuniecia - jest potrzebny gdyz z niego wydobywamy pierwsza wlasciwosc z id w nazwie</param>
        /// <returns>Bool - czy rekord zostal usuniety czy nie</returns>
        public async Task<bool> DeleteTAsync<T>(int ID, T Titem) where T : class
        {
            var nazwaId = PartsOfPrimaryKey(Titem)[0];
            var deletedT = await db.Set<T>().FirstOrDefaultAsync(a => (int)a.GetType().GetProperty(nazwaId).GetValue(a) == ID);

            if (deletedT != null)
            {
                db.Set<T>().Remove(deletedT);
                await db.SaveChangesAsync();
            }

            return (db.Set<T>().FirstOrDefault(a => (int)a.GetType().GetProperty(nazwaId).GetValue(a) == ID) == null);
        }

        /// <summary>
        /// Metoda zwracajaca nazwy wlasciwosci obiektu
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="obiektT">Obiekt z ktorego chcemy pobrac nazwy wlasciwosci</param>
        /// <returns>Liste nazw wlasciwosci obiektu o wybranej klasie</returns>
        public List<string> ListOfProperties<T>(T obiektT) where T : class
        {
            List<string> listWlasci = new List<string>();

            foreach (var item in obiektT.GetType().GetProperties())
            {
                listWlasci.Add(item.Name);
            }

            return listWlasci;
        }

        /// <summary>
        /// Metoda zwracajacy pusty obiekt o wybranej po nazwie klasie
        /// </summary>
        /// <param name="nazwaTabeli">Nazwa klasy jaka ma posiadac nowo stworzony pusty obiekt</param>
        /// <returns>Pusty obiekt o zadanej klasie</returns>
        public dynamic Obiekt(string nazwaTabeli) => Activator.CreateInstance(Type.GetType($"SGGWPZ.Models.{nazwaTabeli}"));

        /// <summary>
        /// Pobierz klucze alternatywne obiektu
        /// </summary>
        /// <typeparam name="T1">Dowolna klasa z folderu Models</typeparam>
        /// <param name="obiekt">Obiekt z ktorego mamy pobrac klucze alternatywne</param>
        /// <returns>Liste kluczy alternatywnych</returns>
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

        /// <summary>
        /// Pobierz wszystkie rekordy z tabeli jako liste obiektow o klasie reprezentujacej tabele
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="item">Obiekt wskazujacy na rodzaj tabeli do pobrania</param>
        /// <returns>Liste obiektow reprezentujaca liste rekordow z tabeli</returns>
        public List<T> ReadAllT<T>(T item) where T : class
        {
            //throw new NotImplementedException();
            return db.Set<T>().ToList();
        }

        /// <summary>
        /// Pobierz asynchronicznie wszystkie rekordy z tabeli jako liste obiektow o klasie reprezentujacej tabele
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="item">Obiekt wskazujacy na rodzaj tabeli do pobrania</param>
        /// <returns>Liste obiektow reprezentujaca liste rekordow z tabeli</returns>
        public async Task<List<T>> ReadAllTAsync<T>(T item) where T : class
        {
            return await db.Set<T>().AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Pobierz rekord(jako Obiekt) o zadanym ID z wybranej tabeli
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="item">Obiekt wskazujacy z ktorej tabeli pobrac wybrany rekord</param>
        /// <returns>Obiekt reprezentujacy rekord z tabeli</returns>
        public async Task<T> ReadTAsync<T>(int ID, T item) where T : class //bylo bez string nazwaId
        {
            var nazwaId = PartsOfPrimaryKey(item)[0];
            return await db.Set<T>().AsNoTracking().FirstOrDefaultAsync(i => (int)i.GetType().GetProperty(nazwaId).GetValue(i) == ID); // bylo Id
        }

        /// <summary>
        /// Pobierz wszystkie rekordy z tabeli jako liste obiektow o klasie reprezentujacej tabele - posortowane wg wlasciowsci
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="PoCzymSortuj">Po jakiej wlasciwosci(kolumnie) posortowac tabele</param>
        /// <param name="item">Obiekt wskazujacy na rodzaj tabeli do pobrania</param>
        /// <returns>Liste posortowanych obiektow reprezentujaca liste rekordow z tabeli po wybranej wlasciwosci</returns>
        public List<T1> SortujPo<T1>(string PoCzymSortuj, T1 item) where T1 : class
        {
            try { return ReadAllT(item).OrderBy(i => i.GetType().GetProperty(PoCzymSortuj).GetValue(i)).ToList(); }
            catch (Exception) { return ReadAllT(item); }
        }

        /// <summary>
        /// Sprawdz czy wybrany obiekt istnieje w tabeli
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="obiekt">Obiekt ktory jest sprawdzany czy istnieje w tabeli</param>
        /// <returns>obiekt jesli istnieje w bazie -- null jesli obiektu nie ma w bazie</returns>
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

        /// <summary>
        /// Edytuj obiekt z tabeli
        /// </summary>
        /// <typeparam name="T">Dowolna klasa z folderu Models</typeparam>
        /// <param name="updatedT">Obiekt ktory zostal zedytowany</param>
        /// <returns>Zedytowany obiekt</returns>
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
            throw new NotImplementedException();

            //if (PoCzym == null || PoCzym == "")
            //    return ReadAllT(item);
            //else
            //{
            //    List<T> akcj = ReadAllT(item);

            //    if (akcj.Where(a => (string)a.GetType().GetProperty(PoCzym).GetValue(a) == Nazwa).ToList().Count != 0)
            //        return akcj.Where(a => (string)a.GetType().GetProperty(PoCzym).GetValue(a) == Nazwa).ToList();
            //    else
            //        return ReadAllT(item);
            //}
        }
    }
}

