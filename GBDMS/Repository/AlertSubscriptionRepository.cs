using GBDMS.Data;
using GBDMS.Models;
using GBDMS.Repository.IRepository;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class AlertSubscriptionRepository : IAlertSubscriptionRepository
    {
        private readonly LocalDbService _dbService;

        public AlertSubscriptionRepository(LocalDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<int> AddSubscriptionAsync(AlertSubscription subscription)
        {
            var database = await _dbService.GetDatabaseAsync();
            
            // Check if subscription already exists
            var existing = await GetSubscriptionByEmailAndDistrictAsync(subscription.Email, subscription.District);
            if (existing != null)
            {
                // Reactivate if it was deactivated
                existing.IsActive = true;
                existing.SubscribedAt = DateTime.Now;
                return await UpdateSubscriptionAsync(existing);
            }
            
            return await database.InsertAsync(subscription);
        }

        public async Task<List<AlertSubscription>> GetActiveSubscriptionsAsync()
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<AlertSubscription>()
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<List<AlertSubscription>> GetSubscriptionsByDistrictAsync(string district)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<AlertSubscription>()
                .Where(s => s.IsActive && (s.District == district || s.District == "all"))
                .ToListAsync();
        }

        public async Task<AlertSubscription?> GetSubscriptionByEmailAndDistrictAsync(string email, string district)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.Table<AlertSubscription>()
                .Where(s => s.Email == email && s.District == district)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateSubscriptionAsync(AlertSubscription subscription)
        {
            var database = await _dbService.GetDatabaseAsync();
            return await database.UpdateAsync(subscription);
        }

        public async Task<int> DeactivateSubscriptionAsync(int id)
        {
            var database = await _dbService.GetDatabaseAsync();
            var subscription = await database.Table<AlertSubscription>()
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            
            if (subscription != null)
            {
                subscription.IsActive = false;
                return await database.UpdateAsync(subscription);
            }
            
            return 0;
        }

        public async Task<bool> IsEmailSubscribedAsync(string email, string district)
        {
            var database = await _dbService.GetDatabaseAsync();
            var subscription = await database.Table<AlertSubscription>()
                .Where(s => s.Email == email && s.District == district && s.IsActive)
                .FirstOrDefaultAsync();
            
            return subscription != null;
        }
    }
}
