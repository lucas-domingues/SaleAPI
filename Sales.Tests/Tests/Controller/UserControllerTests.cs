using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers;
using Sales.API.Interfaces.Services;
using Sales.API.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sales.API.Models;

namespace Sales.Tests.Tests.Controller
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithUsers()
        {
            var paginatedResult = new PaginatedResult<User>
            {
                Data = new List<User> { new User { Id = 1, Username = "TestUser" } },
                TotalItems = 1
            };

            _mockUserService
                .Setup(s => s.GetAllUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(paginatedResult);

            var result = await _controller.GetAllUsers(1, 10, "username asc") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);

            var response = result.Value as PaginatedResult<User>;
            Assert.NotNull(response);
            Assert.NotEmpty(response.Data);
            Assert.Equal(1, response.TotalItems);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsOkResult()
        {
            var user = new User { Id = 1, Username = "TestUser" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _controller.GetUserById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            _mockUserService.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync((User)null);
            var result = await _controller.GetUserById(1);
            Assert.IsType<NotFoundResult>(result);
        }
    }

}