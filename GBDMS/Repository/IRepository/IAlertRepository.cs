using GBDMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Repository.IRepository
{
    public interface IAlertRepository
    {
        Task<int> AddAlertAsync(DisasterAlert alert);
        Task<List<DisasterAlert>> GetAllAsync();
        Task<List<DisasterAlert>> GetActiveAlertsAsync();
        Task<List<DisasterAlert>> GetAlertsByDistrictAsync(string district);
        Task<List<DisasterAlert>> GetAlertsByTypeAsync(string type);
        Task<DisasterAlert?> GetAlertByIdAsync(int id);
        Task<int> UpdateAlertAsync(DisasterAlert alert);
        Task<int> DeactivateAlertAsync(int id);
        Task<List<DisasterAlert>> GetRecentAlertsAsync(int hours = 24);
        Task<int> DeleteExpiredAlertsAsync(int daysOld = 30);
    }
}
