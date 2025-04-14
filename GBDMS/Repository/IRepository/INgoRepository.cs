using System.Collections.Generic;
using System.Threading.Tasks;
using GBDMS.Data;

namespace GBDMS.Repository.IRepository
{
    public interface INgoRepository
    {
        Task<NgoEntity> CreateAsync(NgoEntity ngo);
        Task<bool> DeleteAsync(int id);
        Task<NgoEntity?> GetAsync(int id);
        Task<IEnumerable<NgoEntity>> GetAllAsync();
        Task<NgoEntity?> UpdateAsync(NgoEntity ngo);
        Task<IEnumerable<NgoEntity>> GetNgosByTypeAsync(string type);
        Task<IEnumerable<NgoEntity>> GetNgosByDistrictAsync(string district);
        Task<IEnumerable<NgoEntity>> GetNgosByFocusAreaAsync(string focusArea);
    }
} 