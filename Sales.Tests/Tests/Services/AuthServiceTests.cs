using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Interfaces.Services;
using Sales.API.Models.DTOs;
using Sales.API.Models.Responses;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sales.API.Services;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Models.Entities;


namespace Sales.Tests.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly SalesDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SalesDbContext(options);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("85305acc66e221798f502dd98ee70cdb07dd158ca52c73790c66bdf114658f3b");
            _configuration = mockConfig.Object;

            _authService = new AuthService(_context, _configuration);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            var loginDto = new LoginRequestDto { Username = "testuser", Password = "password" };
            var user = new User
            {
                Username = "testuser",
                Password = "password",
                Email = "testuser@example.com",
                Phone = "1234567890"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = await _authService.AuthenticateAsync(loginDto);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidCredentials_ReturnsNull()
        {
            var loginDto = new LoginRequestDto { Username = "invalid", Password = "wrongpassword" };

            var result = await _authService.AuthenticateAsync(loginDto);
            Assert.Null(result);
        }
    }
}
