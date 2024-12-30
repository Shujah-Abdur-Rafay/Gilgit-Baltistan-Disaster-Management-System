using System.Collections.Generic;
using System.Threading.Tasks;
using GBDMS.Data;

namespace GBDMS.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User?> GetAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> UpdateAsync(User user);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> LoginAsync(string email, string password);
    }
}