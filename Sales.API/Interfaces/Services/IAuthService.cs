using Sales.API.Models.DTOs;

namespace Sales.API.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginRequestDto loginRequest);
    }
}
