using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Extensions;
using Sales.API.Models;
using Sales.API.Models.Entities;
using Sales.API.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sales.API.Services;

public class ProductService : IProductService
{
    private readonly SalesDbContext _context;

    public ProductService(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<Product>> GetProductsAsync(int page, int size, string order)
    {
        try
        {


            var query = _context.Products.Include(p => p.Rating).AsQueryable();

            query = query.ApplySorting(order);


            var totalItems = await query.CountAsync();
            var paginatedItems = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return new PaginatedResult<Product>
            {
                Data = paginatedItems,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / size)
            };
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        try
        {
            return await _context.Products.Include(p => p.Rating).FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<Product> UpdateProductAsync(int id, Product product)
    {
        try
        {
            var existingProduct = await _context.Products
            .Include(p => p.Rating)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null) return null;

            existingProduct.Title = product.Title;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Image = product.Image;


            if (existingProduct.Rating != null && product.Rating != null)
            {
                existingProduct.Rating.Rate = product.Rating.Rate;
                existingProduct.Rating.Count = product.Rating.Count;
                _context.Entry(existingProduct.Rating).State = EntityState.Modified;
            }
            else if (product.Rating != null)
            {
                existingProduct.Rating = product.Rating;
                _context.Entry(existingProduct.Rating).State = EntityState.Added;
            }

            await _context.SaveChangesAsync();
            return existingProduct;
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            return await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }

    public async Task<PaginatedResult<Product>> GetProductsByCategoryAsync(string category, int page, int size, string order)
    {
        try
        {
            if (page < 1) page = 1;
            if (size <= 0) size = 10;

            var query = _context.Products
                .Where(p => EF.Functions.Like(p.Category, category))
                .AsQueryable();

            query = query.ApplySorting(order);

            var totalItems = await query.CountAsync();
            var paginatedItems = totalItems > 0 ? await query.Skip((page - 1) * size).Take(size).ToListAsync() : new List<Product>();

            return new PaginatedResult<Product>
            {
                Data = paginatedItems,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / size)
            };
        }
        catch (Exception ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            throw new Exception($"An error occurred: {ex.Message}; {innerException}");
        }
    }
}
