using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sales.API.Controllers;
using Sales.API.Interfaces.Services;
using Sales.API.Models.Entities;
using Sales.API.Models;
using Sales.API.Models.Responses;
using Xunit;


namespace Sales.Tests.Tests.Controller
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly CartController _cartController;

        public CartControllerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _cartController = new CartController(_mockCartService.Object);
        }

        [Fact]
        public async Task GetAllCarts_ReturnsOkResult_WithPaginatedCarts()
        {
            // Arrange
            var paginatedResult = new PaginatedResult<Cart>
            {
                Data = new List<Cart> { new Cart { Id = 1 } },
                TotalItems = 1
            };

            _mockCartService.Setup(s => s.GetAllCartsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _cartController.GetAllCarts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(paginatedResult, okResult.Value);
        }

        [Fact]
        public async Task GetCartById_CartExists_ReturnsOkResult()
        {
            // Arrange
            var cart = new Cart { Id = 1 };
            _mockCartService.Setup(s => s.GetCartByIdAsync(1)).ReturnsAsync(cart);

            // Act
            var result = await _cartController.GetCartById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(cart, okResult.Value);
        }

        [Fact]
        public async Task GetCartById_CartDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockCartService.Setup(s => s.GetCartByIdAsync(1)).ReturnsAsync((Cart)null);

            // Act
            var result = await _cartController.GetCartById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateCart_ValidCart_ReturnsCreatedAtAction()
        {
            // Arrange
            var cart = new Cart { Id = 1 };
            _mockCartService.Setup(s => s.CreateCartAsync(It.IsAny<Cart>())).ReturnsAsync(cart);

            // Act
            var result = await _cartController.CreateCart(cart);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(cart, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateCart_CartExists_ReturnsOkResult()
        {
            // Arrange
            var cart = new Cart { Id = 1 };
            _mockCartService.Setup(s => s.UpdateCartAsync(1, It.IsAny<Cart>())).ReturnsAsync(cart);

            // Act
            var result = await _cartController.UpdateCart(1, cart);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(cart, okResult.Value);
        }

        [Fact]
        public async Task UpdateCart_CartDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockCartService.Setup(s => s.UpdateCartAsync(1, It.IsAny<Cart>())).ReturnsAsync((Cart)null);

            // Act
            var result = await _cartController.UpdateCart(1, new Cart());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteCart_CartExists_ReturnsNoContent()
        {
            // Arrange
            _mockCartService.Setup(s => s.DeleteCartAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _cartController.DeleteCart(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCart_CartDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockCartService.Setup(s => s.DeleteCartAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _cartController.DeleteCart(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}