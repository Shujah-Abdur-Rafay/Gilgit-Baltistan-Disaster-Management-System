using GBDMS.Data;
using GBDMS.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBDMS.Repository
{
    public class UserRepository : IUserRepository
    {
       private readonly LocalDbService _localDbService;

        public UserRepository(LocalDbService localDbService)
        {
            _localDbService = localDbService;
        }

        public async Task<User> CreateAsync(User obj)
        {
            await _localDbService.GetConnection().InsertAsync(obj);
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
          var obj = await _localDbService.GetConnection().FindAsync<User>(id);
            if (obj == null) return false;

            await _localDbService.GetConnection().DeleteAsync(obj);
            return true;
        }

        public async Task<User?> GetAsync(int id)
        {
            return await _localDbService.GetConnection().FindAsync<User>(id);
        }

       public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _localDbService.GetConnection().Table<User>().ToListAsync();
        }

       public async Task<User?> UpdateAsync(User obj)
        {
            var objFromDb = await _localDbService.GetConnection().FindAsync<User>(obj.Id);
            if (objFromDb == null) return null;

            objFromDb.Username = obj.Username;
            objFromDb.Email = obj.Email;
            objFromDb.Password = obj.Password;
            objFromDb.Role = obj.Role;
            objFromDb.IsActive = obj.IsActive;

           await _localDbService.GetConnection().UpdateAsync(objFromDb);
            return objFromDb;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _localDbService.GetConnection()
                .Table<User>()
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
        {
            return await _localDbService.GetConnection()
                .Table<User>()
               .Where(u => u.Username == username)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _localDbService.GetConnection()
               .Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _localDbService.GetConnection()
                .Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

           if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }
    }
}

