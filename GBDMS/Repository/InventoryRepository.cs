using GBDMS.Data;
using GBDMS.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly LocalDbService _localDbService;

        public InventoryRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<InventoryItem> CreateAsync(InventoryItem obj)
        {
            await _localDbService.GetConnection().InsertAsync(obj);
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = await _localDbService.GetConnection().FindAsync<InventoryItem>(id);
            if (obj == null) return false;

            await _localDbService.GetConnection().DeleteAsync(obj);
            return true;
        }

        public async Task<InventoryItem> GetAsync(int id)
        {
            return await _localDbService.GetConnection().FindAsync<InventoryItem>(id);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _localDbService.GetConnection().Table<InventoryItem>().ToListAsync();
        }

        public async Task<InventoryItem> UpdateAsync(InventoryItem obj)
        {
            var objFromDb = await _localDbService.GetConnection().FindAsync<InventoryItem>(obj.Id);
            if (objFromDb == null) return null;

            objFromDb.Name = obj.Name;
            objFromDb.Category = obj.Category;
            objFromDb.Quantity = obj.Quantity;
            objFromDb.Unit = obj.Unit;
            objFromDb.MinimumLevel = obj.MinimumLevel;
            objFromDb.LastUpdated = obj.LastUpdated;
            objFromDb.District = obj.District;

            await _localDbService.GetConnection().UpdateAsync(objFromDb);
            return objFromDb;
        }

        public async Task<IEnumerable<InventoryItem>> GetItemsByCategoryAsync(string category)
        {
            return await _localDbService.GetConnection()
                .Table<InventoryItem>()
                .Where(i => i.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetItemsByDistrictAsync(string district)
        {
            return await _localDbService.GetConnection()
                .Table<InventoryItem>()
                .Where(i => i.District == district)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetItemsByDistrictAndCategoryAsync(string district, string category)
        {
            return await _localDbService.GetConnection()
                .Table<InventoryItem>()
                .Where(i => i.District == district && i.Category == category)
                .ToListAsync();
        }
    }
}