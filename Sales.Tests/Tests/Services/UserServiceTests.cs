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
    public class UserServiceTests
    {
        private readonly Mock<SalesDbContext> _mockContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<SalesDbContext>(options);
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com" }
            };
            _mockContext.Setup(m => m.Users).ReturnsDbSet(users);
            _userService = new UserService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserAndSaveChanges()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com" };


            var mockSet = new List<User> { user }.AsQueryable().CreateMockDbSet();
            _mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserAndSaveChanges()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            var mockDbSet = new Mock<DbSet<User>>();

            _mockContext.Setup(m => m.Users.FindAsync(1)).ReturnsAsync(user);
            _mockContext.Setup(m => m.Users.Remove(user)).Returns(mockDbSet.Object.Remove(user));
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Users.FindAsync(1), Times.Once);
            _mockContext.Verify(m => m.Users.Remove(user), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            _mockContext.Setup(m => m.Users.FindAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Users.FindAsync(1), Times.Once);
            _mockContext.Verify(m => m.Users.Remove(It.IsAny<User>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnPaginatedResult()
        {

            // Act
            var result = await _userService.GetAllUsersAsync(1, 10, "username");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            var mockDbSet = new Mock<DbSet<User>>();

            _mockContext.Setup(m => m.Users.FindAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(m => m.Users.FindAsync(3)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByIdAsync(3);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserAndSaveChanges()
        {
            // Arrange
            var existingUser = new User { Id = 1, Username = "olduser", Email = "old@example.com" };
            var updatedUser = new User { Id = 1, Username = "newuser", Email = "new@example.com" };

            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUserAsync(1, updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedUser.Username, result.Username);
            Assert.Equal(updatedUser.Email, result.Email);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var result = await _userService.UpdateUserAsync(3, new User());

            // Assert
            Assert.Null(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
