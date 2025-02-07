using Sales.API.Models;
using Sales.API.Models.Entities;

namespace Sales.API.Interfaces.Services
{
    public interface IUserService
    {
        Task<PaginatedResult<User>> GetAllUsersAsync(int page, int size, string order);
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
    }
}
