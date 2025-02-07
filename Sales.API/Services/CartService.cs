using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Events;
using Sales.API.Extensions;
using Sales.API.Interfaces.Services;
using Sales.API.Models;
using Sales.API.Models.Entities;
using Sales.API.Publishers;

namespace Sales.API.Services
{
    public class CartService : ICartService
    {
        private readonly SalesDbContext _context;
        private readonly IEventPublisher _eventPublisher;

        public CartService(SalesDbContext context, IEventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher;
        }

        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            try
            {
                cart = CalculateCartTotal(cart);
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                var saleCreatedEvent = new SaleCreatedEvent(cart.Id, DateTime.UtcNow, cart.TotalPrice);
                await _eventPublisher.PublishSaleCreatedEvent(saleCreatedEvent);
                return cart;
            }
            catch(Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<bool> DeleteCartAsync(int id)
        {
            try
            {


                var cart = await _context.Carts
                    .Include(c => c.Products) // Inclui os produtos relacionados ao carrinho
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cart == null)
                {
                    return false;
                }

                // Remover produtos associados, se necessário
                if (cart.Products != null && cart.Products.Any())
                {
                    cart.Products.Clear();
                }

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                var saleCancelledEvent = new SaleCancelledEvent(cart.Id, DateTime.UtcNow);
                await _eventPublisher.PublishSaleCancelledEvent(saleCancelledEvent);
                return true;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }   
        }


        public async Task<PaginatedResult<Cart>> GetAllCartsAsync(int page, int size, string order)
        {
            try
            {


                var query = _context.Carts.Include(c => c.Products).AsQueryable();

                query = query.ApplySorting(order);


                var totalItems = await query.CountAsync();
                var paginatedItems = await query.Skip((page - 1) * size).Take(size).ToListAsync();

                return new PaginatedResult<Cart>
                {
                    Data = paginatedItems,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalItems / size)
                };
            }
            catch(Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<Cart> GetCartByIdAsync(int id)
        {
            try
            {
                return await _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            }
            catch(Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                throw new Exception($"An error occurred: {ex.Message}; {innerException}");
            }
        }

        public async Task<Cart> UpdateCartAsync(int id, Cart cart)
        {
            try
            {
                var existingCart = await _context.Carts
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingCart == null) return null;

                // Atualizando dados principais
                existingCart.UserId = cart.UserId;
                existingCart.Date = cart.Date.ToUniversalTime();

                // Verificando se a lista de produtos foi alterada
                if (cart.Products != null)
                {
                    // Remover os produtos que não estão mais no carrinho
                    var productsToRemove = existingCart.Products
                        .Where(p => !cart.Products.Any(cp => cp.ProductId == p.ProductId))
                        .ToList();

                    foreach (var product in productsToRemove)
                    {
                        existingCart.Products.Remove(product);
                    }

                    // Adicionar novos produtos ou atualizar os existentes
                    foreach (var newProduct in cart.Products)
                    {
                        var existingProduct = existingCart.Products
                            .FirstOrDefault(p => p.ProductId == newProduct.ProductId);

                        if (existingProduct != null)
                        {
                            // Atualiza a quantidade do produto existente
                            existingProduct.Quantity = newProduct.Quantity;
                        }
                        else
                        {
                            // Adiciona um novo produto
                            existingCart.Products.Add(newProduct);

                            _context.Entry(newProduct).State = EntityState.Added;
                        }
                    }
                }
                existingCart = CalculateCartTotal(existingCart);

                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();

                var saleModifiedEvent = new SaleModifiedEvent(existingCart.Id, DateTime.UtcNow, existingCart.TotalPrice);
                await _eventPublisher.PublishSaleModifiedEvent(saleModifiedEvent);
                return existingCart;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while updating the cart", ex);
            }
        }

        public Cart CalculateCartTotal(Cart cart)
        {
            foreach (var cartProduct in cart.Products)
            {
                // Fetch the product details
                var product = _context.Products.FirstOrDefault(p => p.Id == cartProduct.ProductId);
                if (product == null)
                    throw new Exception("Product not found.");

                // Validate quantity restrictions
                if (cartProduct.Quantity > 20)
                    throw new Exception($"Cannot purchase more than 20 units of product {product.Title}.");

                // Apply discount tiers
                if (cartProduct.Quantity >= 4 && cartProduct.Quantity < 10)
                {
                    cartProduct.Discount = 0.10m; // 10% discount
                }
                else if (cartProduct.Quantity >= 10 && cartProduct.Quantity <= 20)
                {
                    cartProduct.Discount = 0.20m; // 20% discount
                }
                else
                {
                    cartProduct.Discount = 0.00m; // No discount
                }

                // Calculate total price for the product
                cartProduct.TotalPrice = cartProduct.Quantity * product.Price * (1 - cartProduct.Discount);
            }

            // Calculate total cart price
            cart.TotalPrice = cart.Products.Sum(p => p.TotalPrice);
            return cart;
        }

    }
}
