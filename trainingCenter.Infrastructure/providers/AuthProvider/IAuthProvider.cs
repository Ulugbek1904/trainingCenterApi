using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using trainingCenter.Domain.Models;

namespace trainingCenter.Infrastructure.providers.AuthProvider
{
    public interface IAuthProvider
    {
        string GenerateJwtToken(User user);
        (string Token, DateTime Expiration) GenerateRefreshToken();
        TokenValidationParameters GetValidationParameters();
        ClaimsPrincipal ValidateToken(string token);
    }
}