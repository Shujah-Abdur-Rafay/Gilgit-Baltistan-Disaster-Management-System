using GBDMS.Data;
using GBDMS.Models;
using GBDMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class SurvivalGuidelineRepository : ISurvivalGuidelineRepository
    {
        private readonly LocalDbService _localDbService;

        public SurvivalGuidelineRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<SurvivalGuideline> CreateAsync(SurvivalGuideline guideline)
        {
            guideline.CreatedAt = DateTime.Now;
            guideline.UpdatedAt = DateTime.Now;
            await _localDbService.GetConnection().InsertAsync(guideline);
            return guideline;
        }

        public async Task<SurvivalGuideline> GetAsync(int id)
        {
            return await _localDbService.GetConnection().FindAsync<SurvivalGuideline>(id);
        }

        public async Task<List<SurvivalGuideline>> GetAllAsync()
        {
            return await _localDbService.GetConnection().Table<SurvivalGuideline>().ToListAsync();
        }

        public async Task<List<SurvivalGuideline>> GetActiveAsync()
        {
            return await _localDbService.GetConnection()
                .Table<SurvivalGuideline>()
                .Where(g => g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<List<SurvivalGuideline>> GetByCategoryAsync(string category)
        {
            return await _localDbService.GetConnection()
                .Table<SurvivalGuideline>()
                .Where(g => g.IsActive && g.Category == category)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<SurvivalGuideline> UpdateAsync(SurvivalGuideline guideline)
        {
            guideline.UpdatedAt = DateTime.Now;
            await _localDbService.GetConnection().UpdateAsync(guideline);
            return guideline;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var guideline = await _localDbService.GetConnection().FindAsync<SurvivalGuideline>(id);
            if (guideline == null) return false;

            await _localDbService.GetConnection().DeleteAsync(guideline);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var guideline = await _localDbService.GetConnection().FindAsync<SurvivalGuideline>(id);
            if (guideline == null) return false;

            guideline.IsActive = false;
            guideline.UpdatedAt = DateTime.Now;
            await _localDbService.GetConnection().UpdateAsync(guideline);
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var guideline = await _localDbService.GetConnection().FindAsync<SurvivalGuideline>(id);
            if (guideline == null) return false;

            guideline.IsActive = true;
            guideline.UpdatedAt = DateTime.Now;
            await _localDbService.GetConnection().UpdateAsync(guideline);
            return true;
        }
    }
}
