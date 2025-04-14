using GBDMS.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public interface IDamageRepository
    {
        Task<List<DamageRecord>> GetAllAsync();
        Task<DamageRecord> GetByIdAsync(int id);
        Task<List<DamageRecord>> GetByDisasterTypeAsync(string type);
        Task<List<DamageRecord>> GetByDistrictAsync(string district);
        Task<List<DamageRecord>> GetByCategoryAsync(string category);
        Task<List<DamageRecord>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<DamageRecord> CreateAsync(DamageRecord damage);
        Task<bool> UpdateAsync(DamageRecord damage);
        Task<bool> DeleteAsync(int id);
    }

    public class DamageRepository : IDamageRepository
    {
        private readonly LocalDbService _localDbService;

        public DamageRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<List<DamageRecord>> GetAllAsync()
        {
            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .ToListAsync();
        }

        public async Task<DamageRecord> GetByIdAsync(int id)
        {
            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DamageRecord>> GetByDisasterTypeAsync(string type)
        {
            if (string.IsNullOrEmpty(type))
                return await GetAllAsync();

            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .Where(d => d.DisasterType == type)
                .ToListAsync();
        }

        public async Task<List<DamageRecord>> GetByDistrictAsync(string district)
        {
            if (string.IsNullOrEmpty(district))
                return await GetAllAsync();

            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .Where(d => d.District == district)
                .ToListAsync();
        }

        public async Task<List<DamageRecord>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrEmpty(category))
                return await GetAllAsync();

            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .Where(d => d.Category == category)
                .ToListAsync();
        }

        public async Task<List<DamageRecord>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _localDbService.GetConnection()
                .Table<DamageRecord>()
                .Where(d => d.Date >= start && d.Date <= end)
                .ToListAsync();
        }

        public async Task<DamageRecord> CreateAsync(DamageRecord damage)
        {
            await _localDbService.GetConnection().InsertAsync(damage);
            return damage;
        }

        public async Task<bool> UpdateAsync(DamageRecord damage)
        {
            var result = await _localDbService.GetConnection().UpdateAsync(damage);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _localDbService.GetConnection().DeleteAsync<DamageRecord>(id);
            return result > 0;
        }
    }
} 