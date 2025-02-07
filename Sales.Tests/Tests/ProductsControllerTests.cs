using Microsoft.AspNetCore.Mvc;
using Moq;
using Sales.API.Controllers;
using Sales.API.Models;
using Sales.API.Models.Entities;
using Sales.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductsController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithProducts()
        {
            var paginatedResult = new PaginatedResult<Product>
            {
                Data = new List<Product> { new Product { Id = 1, Title = "Product A" } },
                TotalItems = 1
            };

            _mockProductService
                .Setup(s => s.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(paginatedResult);

            var result = await _controller.GetProducts(1, 10, null) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            Assert.NotNull(result.Value);

            var response = result.Value as PaginatedResult<Product>;
            Assert.NotNull(response);
            Assert.NotEmpty(response.Data);
            Assert.Equal(1, response.TotalItems);
        }

        [Fact]
        public async Task GetProductById_ProductExists_ReturnsOkResult()
        {
            var product = new Product { Id = 1, Title = "TestProduct" };
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.GetProductById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ReturnsNotFound()
        {
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((Product)null);
            var result = await _controller.GetProductById(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
