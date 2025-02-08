using Microsoft.EntityFrameworkCore;
using Moq;
using Sales.API.Data;
using Sales.API.Events;
using Sales.API.Models.Entities;
using Sales.API.Publishers;
using Sales.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.Tests.Services
{
    public class CartServiceTests
    {
        private readonly CartService _cartService;
        private readonly SalesDbContext _context;
        private readonly Mock<IEventPublisher> _eventPublisherMock;

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SalesDbContext(options);
            _eventPublisherMock = new Mock<IEventPublisher>();

            _cartService = new CartService(_context, _eventPublisherMock.Object);
        }

        [Fact]
        public async Task CreateCartAsync_ShouldCreateCartAndPublishEvent()
        {
            var cart = new Cart { UserId = 1, Date = DateTime.UtcNow, Products = new List<CartProduct>() };

            var result = await _cartService.CreateCartAsync(cart);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            _eventPublisherMock.Verify(e => e.PublishSaleCreatedEvent(It.IsAny<SaleCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetCartByIdAsync_ShouldReturnCart_WhenCartExists()
        {
            var cart = new Cart { UserId = 1, Date = DateTime.UtcNow, Products = new List<CartProduct>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            var result = await _cartService.GetCartByIdAsync(cart.Id);
            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
        }

        [Fact]
        public async Task DeleteCartAsync_ShouldDeleteCartAndPublishEvent()
        {
            var cart = new Cart { UserId = 1, Date = DateTime.UtcNow, Products = new List<CartProduct>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            var result = await _cartService.DeleteCartAsync(cart.Id);
            Assert.True(result);
            _eventPublisherMock.Verify(e => e.PublishSaleCancelledEvent(It.IsAny<SaleCancelledEvent>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCartAsync_ShouldUpdateCart()
        {
            var cart = new Cart { UserId = 1, Date = DateTime.UtcNow, Products = new List<CartProduct>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            cart.UserId = 2;
            var updatedCart = await _cartService.UpdateCartAsync(cart.Id, cart);

            Assert.NotNull(updatedCart);
            Assert.Equal(2, updatedCart.UserId);
        }
    }
}
