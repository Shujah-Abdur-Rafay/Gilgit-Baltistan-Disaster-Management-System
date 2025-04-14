using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GBDMS.Data;
using SQLite;

namespace GBDMS.Repository
{
    public interface IIncidentRepository
    {
        Task<List<Incident>> GetAllAsync();
        Task<Incident> GetByIdAsync(int id);
        Task<List<Incident>> GetByLocationAsync(string location);
        Task<List<Incident>> GetByTypeAsync(string type);
        Task<Incident> CreateAsync(Incident incident);
        Task<bool> UpdateAsync(Incident incident);
        Task<bool> DeleteAsync(int id);
    }

    public class IncidentRepository : IIncidentRepository
    {
        private readonly LocalDbService _localDbService;

        public IncidentRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<List<Incident>> GetAllAsync()
        {
            return await _localDbService.GetConnection()
                .Table<Incident>()
                .ToListAsync();
        }

        public async Task<Incident> GetByIdAsync(int id)
        {
            return await _localDbService.GetConnection()
                .Table<Incident>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Incident>> GetByLocationAsync(string location)
        {
            return await _localDbService.GetConnection()
                .Table<Incident>()
                .Where(i => i.Location == location)
                .ToListAsync();
        }

        public async Task<List<Incident>> GetByTypeAsync(string type)
        {
            return await _localDbService.GetConnection()
                .Table<Incident>()
                .Where(i => i.Type == type)
                .ToListAsync();
        }

        public async Task<Incident> CreateAsync(Incident incident)
        {
            incident.CreatedDate = DateTime.Now;
            await _localDbService.GetConnection().InsertAsync(incident);
            return incident;
        }

        public async Task<bool> UpdateAsync(Incident incident)
        {
            var result = await _localDbService.GetConnection().UpdateAsync(incident);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _localDbService.GetConnection().DeleteAsync<Incident>(id);
            return result > 0;
        }
    }
} 