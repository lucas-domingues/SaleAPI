using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Extensions;
using Sales.API.Interfaces.Services;
using Sales.API.Models.Entities;

namespace Sales.API.Services
{
    public class UserService : IUserService
    {
        private readonly SalesDbContext _context;

        public UserService(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<List<User>> GetAllUsersAsync(int page, int size, string order)
        {
            try
            {


                var query = _context.Users.Include(n => n.Name).Include(a => a.Address).Include(g => g.Address.Geolocation).AsQueryable();

                query = query.ApplySorting(order);
                var users = await query.Skip((page - 1) * size).Take(size).ToListAsync();
                return users;
            }
            catch(Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.Include(u => u.Name).Include(u => u.Address).Include(g => g.Address.Geolocation).FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<User> UpdateUserAsync(int id, User user)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (existingUser == null) return null;

                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Name = user.Name;
                existingUser.Address = user.Address;
                existingUser.Phone = user.Phone;
                existingUser.Status = user.Status;
                existingUser.Role = user.Role;

                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }
    }
}
