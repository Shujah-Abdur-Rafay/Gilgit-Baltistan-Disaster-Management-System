﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public SQLiteAsyncConnection GetConnection() => _connection;
    }
}
