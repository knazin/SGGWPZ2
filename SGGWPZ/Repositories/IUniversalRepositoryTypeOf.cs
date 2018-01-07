using SGGWPZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGGWPZ.Repositories
{
    public interface IUniversalRepositoryTypeOf
    {
        T SprawdzCzyIstniejeWBazie<T>(T obiekt) where T : class;

        Task<T> CreateTAsync<T>(T newT) where T : class;

        Task<bool> DeleteTAsync<T>(int ID, T Titem) where T : class;

        List<T> ReadAllT<T>(T item) where T : class;

        Task<List<T>> ReadAllTAsync<T>(T item) where T : class;

        Task<T> ReadTAsync<T>(int ID, T Tiem) where T : class;

        List<T> SortujPo<T>(string PoCzymSortuj, T Tiem) where T : class;

        Task<T> UpdateTAsync<T>(T updatedT) where T : class;

        List<T> WszystkieOpcje<T>(string PoCzym, string Nazwa, string PoCzymSortuj, T Titem) where T : class;

        List<T> WyszukajPoSymbolu<T>(string PoCzym, string Nazwa, T Tiem) where T : class;

        List<string> ListOfProperties<T>(T obiektT) where T : class;

        List<string> PartsOfPrimaryKey<T>(T obiekt) where T : class;

        List<string> PartsOfAlternativeKey<T>(T obiekt) where T : class;

        //Nowe

        string PolecenieStworzeniaPierwszegoElementuWTablicy(string NazwaTabeli);

        List<dynamic> PobierzTablice(string nazwaTablicy);

        dynamic Obiekt(string nazwaTabeli);
        
    }
}
