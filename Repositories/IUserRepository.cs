using LibraryApp.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApp.Web.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeactivateAsync(string id);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
