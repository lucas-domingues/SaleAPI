using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Controllers;
using Sales.API.Interfaces.Services;
using Sales.API.Models.DTOs;
using Sales.API.Models.Responses;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace Sales.Tests.Tests.Controller
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Username = "testuser", Password = "password123" };
            var expectedToken = "valid_token";

            _authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _authController.Login(loginRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var responseObject = JObject.FromObject(result.Value);
            Assert.Equal(expectedToken, responseObject["token"].ToString());
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Username = "invaliduser", Password = "wrongpassword" };
            _authServiceMock.Setup(s => s.AuthenticateAsync(loginRequest)).ReturnsAsync(string.Empty);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = unauthorizedResult.Value as ApiErrorResponse;
            Assert.Equal("AuthenticationError", response.Type);
            Assert.Equal("Cannot login", response.Error);
        }
    }
}