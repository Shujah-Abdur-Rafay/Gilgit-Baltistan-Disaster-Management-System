using GBDMS.Data;
using GBDMS.Models;
using GBDMS.Repository.IRepository;

namespace GBDMS.Repository
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly LocalDbService _localDbService;

        public ContactMessageRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage obj)
        {
            await _localDbService.GetConnection().InsertAsync(obj);
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = await _localDbService.GetConnection().FindAsync<ContactMessage>(id);
            if (obj == null) return false;

            await _localDbService.GetConnection().DeleteAsync(obj);
            return true;
        }

        public async Task<ContactMessage?> GetAsync(int id)
        {
            return await _localDbService.GetConnection().FindAsync<ContactMessage>(id);
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await _localDbService.GetConnection().Table<ContactMessage>()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContactMessage> UpdateAsync(ContactMessage obj)
        {
            var objFromDb = await _localDbService.GetConnection().FindAsync<ContactMessage>(obj.Id);
            if (objFromDb == null) return null;

            objFromDb.FullName = obj.FullName;
            objFromDb.Email = obj.Email;
            objFromDb.ContactNumber = obj.ContactNumber;
            objFromDb.Remarks = obj.Remarks;
            objFromDb.IsRead = obj.IsRead;
            objFromDb.Status = obj.Status;
            objFromDb.AdminResponse = obj.AdminResponse;
            objFromDb.RespondedAt = obj.RespondedAt;

            await _localDbService.GetConnection().UpdateAsync(objFromDb);
            return objFromDb;
        }

        public async Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync()
        {
            return await _localDbService.GetConnection().Table<ContactMessage>()
                .Where(c => !c.IsRead)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetMessagesByStatusAsync(string status)
        {
            return await _localDbService.GetConnection().Table<ContactMessage>()
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _localDbService.GetConnection().Table<ContactMessage>()
                .Where(c => !c.IsRead)
                .CountAsync();
        }
    }
}
