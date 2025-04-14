using GBDMS.Data;
using GBDMS.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class NgoRepository : INgoRepository
    {
        private readonly LocalDbService _localDbService;

        public NgoRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<NgoEntity> CreateAsync(NgoEntity obj)
        {
            await _localDbService.GetConnection().InsertAsync(obj);
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = await _localDbService.GetConnection().FindAsync<NgoEntity>(id);
            if (obj == null) return false;

            await _localDbService.GetConnection().DeleteAsync(obj);
            return true;
        }

        public async Task<NgoEntity> GetAsync(int id)
        {
            return await _localDbService.GetConnection().FindAsync<NgoEntity>(id);
        }

        public async Task<IEnumerable<NgoEntity>> GetAllAsync()
        {
            return await _localDbService.GetConnection().Table<NgoEntity>().ToListAsync();
        }

        public async Task<NgoEntity> UpdateAsync(NgoEntity obj)
        {
            var objFromDb = await _localDbService.GetConnection().FindAsync<NgoEntity>(obj.Id);
            if (objFromDb == null) return null;

            objFromDb.Name = obj.Name;
            objFromDb.Type = obj.Type;
            objFromDb.District = obj.District;
            objFromDb.Location = obj.Location;
            objFromDb.PrimaryFocusArea = obj.PrimaryFocusArea;
            objFromDb.SecondaryFocusAreasString = obj.SecondaryFocusAreasString;
            objFromDb.ContactPhone = obj.ContactPhone;
            objFromDb.Email = obj.Email;
            objFromDb.Website = obj.Website;
            objFromDb.Description = obj.Description;
            objFromDb.Latitude = obj.Latitude;
            objFromDb.Longitude = obj.Longitude;

            await _localDbService.GetConnection().UpdateAsync(objFromDb);
            return objFromDb;
        }

        public async Task<IEnumerable<NgoEntity>> GetNgosByTypeAsync(string type)
        {
            return await _localDbService.GetConnection()
                .Table<NgoEntity>()
                .Where(n => n.Type == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<NgoEntity>> GetNgosByDistrictAsync(string district)
        {
            return await _localDbService.GetConnection()
                .Table<NgoEntity>()
                .Where(n => n.District == district)
                .ToListAsync();
        }

        public async Task<IEnumerable<NgoEntity>> GetNgosByFocusAreaAsync(string focusArea)
        {
            return await _localDbService.GetConnection()
                .Table<NgoEntity>()
                .Where(n => n.PrimaryFocusArea == focusArea || n.SecondaryFocusAreasString.Contains(focusArea))
                .ToListAsync();
        }
    }
} 