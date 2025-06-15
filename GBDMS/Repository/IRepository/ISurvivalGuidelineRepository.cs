using GBDMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Repository.IRepository
{
    public interface ISurvivalGuidelineRepository
    {
        Task<SurvivalGuideline> CreateAsync(SurvivalGuideline guideline);
        Task<SurvivalGuideline> GetAsync(int id);
        Task<List<SurvivalGuideline>> GetAllAsync();
        Task<List<SurvivalGuideline>> GetActiveAsync();
        Task<List<SurvivalGuideline>> GetByCategoryAsync(string category);
        Task<SurvivalGuideline> UpdateAsync(SurvivalGuideline guideline);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<bool> ActivateAsync(int id);
    }
}
