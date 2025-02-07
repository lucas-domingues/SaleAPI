using Sales.API.Models;
using Sales.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sales.API.Services;

public interface IProductService
{

    Task<PaginatedResult<Product>> GetProductsAsync(int page, int size, string order);
    Task<Product> AddProductAsync(Product product);
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> UpdateProductAsync(int id, Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<List<string>> GetCategoriesAsync();
    Task<PaginatedResult<Product>> GetProductsByCategoryAsync(string category, int page, int size, string order);
}