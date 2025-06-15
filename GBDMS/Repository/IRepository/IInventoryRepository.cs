using System.Collections.Generic;
using System.Threading.Tasks;
using GBDMS.Data;

namespace GBDMS.Repository.IRepository
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> CreateAsync(InventoryItem item);
        Task<bool> DeleteAsync(int id);
        Task<InventoryItem?> GetAsync(int id);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem?> UpdateAsync(InventoryItem item);
        Task<IEnumerable<InventoryItem>> GetItemsByCategoryAsync(string category);
        Task<IEnumerable<InventoryItem>> GetItemsByDistrictAsync(string district);
        Task<IEnumerable<InventoryItem>> GetItemsByDistrictAndCategoryAsync(string district, string category);
    }
}