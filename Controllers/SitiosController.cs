using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using PM0220242P.Models;

namespace PM0220242P.Controllers
{
    public class SitiosController
    {
        readonly SQLiteAsyncConnection _connection;

        public SitiosController()
        {
            SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite |
                SQLite.SQLiteOpenFlags.Create |
                SQLite.SQLiteOpenFlags.SharedCache;

            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBSitios.db3"), extensiones);
            _connection.CreateTableAsync<Models.Sitios>();
        }

        // Crud Methods

        // Create / Update
        public async Task<int> StoreSitio(Models.Sitios sitio)
        {
            if (sitio.Id == 0)
            {
                return await _connection.InsertAsync(sitio);
            }
            else
            {
                return await _connection.UpdateAsync(sitio);
            }
        }

        // Read
        public async Task<List<Models.Sitios>> GetListSitios()
        {
            return await _connection.Table<Models.Sitios>().ToListAsync();
        }

        public async Task<Models.Sitios> GetSitio(int id)
        {
            return await _connection.Table<Models.Sitios>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        // Delete
        public async Task<int> DeleteSitio(Models.Sitios sitio)
        {
            return await _connection.DeleteAsync(sitio);
        }
    }
}
