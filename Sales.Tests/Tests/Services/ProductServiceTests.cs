using Microsoft.EntityFrameworkCore;
using Moq;
using Sales.API.Data;
using Sales.API.Models.Entities;
using Sales.API.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Sales.Tests.TestHelpers;
using Moq.EntityFrameworkCore;

namespace Sales.Tests.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<SalesDbContext> _mockContext;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<SalesDbContext>(options);
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Category = "Category1", Rating = new Rating {Id = 1, Rate = 4.5, Count = 100 } },
                new Product { Id = 2, Title = "Prod2", Description = "Desc2", Image = "Img2.png", Category = "Category2", Rating = new Rating {Id = 2, Rate = 5.0, Count = 200 } },
                new Product { Id = 3, Title = "Prod3", Description = "Desc3", Image = "Img3.png", Category = "Category3", Rating = new Rating {Id = 3, Rate = 3.5, Count = 150 } },
                new Product { Id = 3, Title = "Prod4", Description = "Desc4", Image = "Img4.png", Category = "Category1", Rating = new Rating {Id = 4, Rate = 3.5, Count = 150 } }
            };
            _mockContext.Setup(m => m.Products).ReturnsDbSet(products);
            _productService = new ProductService(_mockContext.Object);

        }

        [Fact]
        public async Task GetProductsAsync_ReturnsPaginatedResult()
        {
            // Act
            var result = await _productService.GetProductsAsync(page: 1, size: 2, order: "title");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.TotalItems); // Total de itens na lista
            Assert.Equal(1, result.CurrentPage); // Página atual
            Assert.Equal(2, result.TotalPages); // Total de páginas (3 itens / 2 por página = 2 páginas)
            Assert.Equal(2, result.Data.Count()); // Itens na página atual
            Assert.Equal("Prod1", result.Data.First().Title); // Verifica a ordenação
        }

        [Fact]
        public async Task AddProductAsync_AddsProductToDatabase()
        {
            // Arrange
            var product = new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Category = "Category1" };


            var mockSet = new List<Product> { product }.AsQueryable().CreateMockDbSet();
            _mockContext.Setup(m => m.Products).Returns(mockSet.Object);
            // Act
            var result = await _productService.AddProductAsync(product);

            // Assert
            Assert.Equal(product, result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Rating = new Rating(), Category = "Category1" },
                new Product { Id = 2, Title = "Prod2", Description = "Desc2", Image = "Img2.png", Rating = new Rating(), Category = "Category2" }
            }.AsQueryable();

            var mockSet = products.CreateMockDbSet();
            _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Prod1", result.Title);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesExistingProduct()
        {
            var updatedProduct = new Product
            {
                Title = "UpdatedTitle",
                Price = 99.99m,
                Description = "UpdatedDesc",
                Category = "UpdatedCategory",
                Image = "UpdatedImg.png",
                Rating = new Rating { Id = 1, Rate = 4.8, Count = 120 }
            };

            var existingProduct = new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Category = "Category1", Rating = new Rating { Id = 1, Rate = 4.5, Count = 100 } };

            var fakeEntityEntry = new FakeEntityEntry<Rating>(existingProduct.Rating);

            _mockContext
                .Setup(m => m.Entry(existingProduct.Rating))
                .Returns(fakeEntityEntry);

            _mockContext
                .Setup(m => m.SaveChangesAsync(default))
                .ReturnsAsync(1);

            var prods = _mockContext.Setup(x => x.Products.FindAsync(1)).ReturnsAsync(existingProduct);

            _mockContext.Setup(m => m.Products).ReturnsDbSet(new List<Product> { existingProduct });



            var result = await _productService.UpdateProductAsync(1, updatedProduct);

            Assert.NotNull(result);
            Assert.Equal("UpdatedTitle", result.Title);
            Assert.Equal(99.99m, result.Price);
            Assert.Equal("UpdatedDesc", result.Description);
            Assert.Equal("UpdatedCategory", result.Category);
            Assert.Equal("UpdatedImg.png", result.Image);
            Assert.NotNull(result.Rating);
            Assert.Equal(4.8, result.Rating.Rate);
            Assert.Equal(120, result.Rating.Count);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Category = "Category1" };

            var mockSet = new List<Product> { product }.AsQueryable().CreateMockDbSet();
            _mockContext.Setup(m => m.Products).Returns(mockSet.Object);
            _mockContext.Setup(m => m.Products.FindAsync(1)).ReturnsAsync(product);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsListOfCategories()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Prod1", Description = "Desc1", Image = "Img1.png", Category = "Category1" },
                new Product { Id = 2,Title = "Prod2", Description = "Desc2", Image = "Img2.png", Category = "Category2" }
            }.AsQueryable();

            var mockSet = products.CreateMockDbSet();

            _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

            var result = await _productService.GetCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("Category1", result);
            Assert.Contains("Category2", result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsPaginatedResult()
        {
            // Act
            var result = await _productService.GetProductsByCategoryAsync("Category1", 1, 10, "title");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(2, result.Data.Count());
        }
    }
}
