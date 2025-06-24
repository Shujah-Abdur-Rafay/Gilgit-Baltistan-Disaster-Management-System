using GBDMS.Models;

namespace GBDMS.Repository.IRepository
{
    public interface IContactMessageRepository
    {
        Task<ContactMessage> CreateAsync(ContactMessage obj);
        Task<ContactMessage?> GetAsync(int id);
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<ContactMessage> UpdateAsync(ContactMessage obj);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync();
        Task<IEnumerable<ContactMessage>> GetMessagesByStatusAsync(string status);
        Task<int> GetUnreadCountAsync();
    }
}
