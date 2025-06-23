using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using trainingCenter.Domain.Models;

namespace trainingCenter.Infrastructure.providers.AuthProvider
{
    public interface IAuthProvider
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        public ClaimsPrincipal ValidateToken(string token);
        TokenValidationParameters GetValidationParameters();
    }
}