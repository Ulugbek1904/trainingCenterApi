using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs.Auth;
using trainingCenter.Domain.Models.DTOs.User;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }

}
