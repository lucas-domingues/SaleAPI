using Sales.API.Models;
using Sales.API.Models.Entities;

namespace Sales.API.Interfaces.Services
{
    public interface ICartService
    {
        Task<PaginatedResult<Cart>> GetAllCartsAsync(int page, int size, string order);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<Cart> GetCartByIdAsync(int id);
        Task<Cart> UpdateCartAsync(int id, Cart cart);
        Task<bool> DeleteCartAsync(int id);
    }
}
