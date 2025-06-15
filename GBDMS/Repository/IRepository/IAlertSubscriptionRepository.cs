using GBDMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GBDMS.Repository.IRepository
{
    public interface IAlertSubscriptionRepository
    {
        Task<int> AddSubscriptionAsync(AlertSubscription subscription);
        Task<List<AlertSubscription>> GetActiveSubscriptionsAsync();
        Task<List<AlertSubscription>> GetSubscriptionsByDistrictAsync(string district);
        Task<AlertSubscription?> GetSubscriptionByEmailAndDistrictAsync(string email, string district);
        Task<int> UpdateSubscriptionAsync(AlertSubscription subscription);
        Task<int> DeactivateSubscriptionAsync(int id);
        Task<bool> IsEmailSubscribedAsync(string email, string district);
    }
}
