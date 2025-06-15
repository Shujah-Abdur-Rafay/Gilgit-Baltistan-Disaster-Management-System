using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBDMS.Models;

namespace GBDMS.Data
{
    public class LocalDbService
    {
        private const string DB_NAME = "gbdms.db3";
        private readonly SQLiteAsyncConnection _connection;
        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<User>();
            _connection.CreateTableAsync<InventoryItem>();
            _connection.CreateTableAsync<Incident>();
            _connection.CreateTableAsync<DamageRecord>();
            _connection.CreateTableAsync<NgoEntity>();
            _connection.CreateTableAsync<AlertSubscription>();
            _connection.CreateTableAsync<DisasterAlert>();
            _connection.CreateTableAsync<SurvivalGuideline>();
        }
        public SQLiteAsyncConnection GetConnection() => _connection;

        public async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            return _connection;
        }
    }
}
