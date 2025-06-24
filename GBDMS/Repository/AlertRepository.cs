using GBDMS.Data;
using GBDMS.Models;
using GBDMS.Repository.IRepository;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly LocalDbService _dbService;

        public AlertRepository(LocalDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<int> AddAlertAsync(DisasterAlert alert)
        {
            var database = await _dbService.GetDatabaseAsync();
            alert.CreatedAt = DateTime.Now;
            alert.Time = DateTime.Now;
            return await database.InsertAsync(alert);
        }

        public async Task<List<DisasterAlert>> GetAllAsync()
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<DisasterAlert>()
                .OrderByDescending(a => a.Time)
                .ToListAsync();
        }

        public async Task<List<DisasterAlert>> GetActiveAlertsAsync()
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<DisasterAlert>()
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.Time)
                .ToListAsync();
        }

        public async Task<List<DisasterAlert>> GetAlertsByDistrictAsync(string district)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<DisasterAlert>()
                .Where(a => a.IsActive && a.Area == district)
                .OrderByDescending(a => a.Time)
                .ToListAsync();
        }

        public async Task<List<DisasterAlert>> GetAlertsByTypeAsync(string type)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<DisasterAlert>()
                .Where(a => a.IsActive && a.Type == type)
                .OrderByDescending(a => a.Time)
                .ToListAsync();
        }

        public async Task<DisasterAlert?> GetAlertByIdAsync(int id)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<DisasterAlert>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAlertAsync(DisasterAlert alert)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.UpdateAsync(alert);
        }

        public async Task<int> DeactivateAlertAsync(int id)
        {
            var database = await _dbService.GetDatabaseAsync();
            var alert = await GetAlertByIdAsync(id);
            if (alert != null)
            {
                alert.IsActive = false;
                return await UpdateAlertAsync(alert);
            }
            return 0;
        }

        public async Task<List<DisasterAlert>> GetRecentAlertsAsync(int hours = 24)
        {
            var database = await _dbService.GetDatabaseAsync();
            var cutoffTime = DateTime.Now.AddHours(-hours);
            return await database.Table<DisasterAlert>()
                .Where(a => a.IsActive && a.Time >= cutoffTime)
                .OrderByDescending(a => a.Time)
                .ToListAsync();
        }

        public async Task<int> DeleteExpiredAlertsAsync(int daysOld = 30)
        {
            var database = await _dbService.GetDatabaseAsync();
            var cutoffDate = DateTime.Now.AddDays(-daysOld);
            var expiredAlerts = await database.Table<DisasterAlert>()
                .Where(a => a.CreatedAt < cutoffDate)
                .ToListAsync();

            int deletedCount = 0;
            foreach (var alert in expiredAlerts)
            {
                await database.DeleteAsync(alert);
                deletedCount++;
            }
            return deletedCount;
        }
    }
}
