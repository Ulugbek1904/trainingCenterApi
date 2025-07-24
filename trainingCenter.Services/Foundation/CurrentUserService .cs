using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Common.Exceptions;

namespace trainingCenter.Infrastructure.services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            Guid.TryParse(GetClaim(ClaimTypes.NameIdentifier), out var id) ? id :
            throw new UnauthorizedException("User ID is missing in token.");

        public Guid TenantId =>
            Guid.TryParse(GetClaim("TenantId"), out var id) ? id :
            throw new UnauthorizedException("Tenant ID is missing in token.");

        public string Role =>
            GetClaim(ClaimTypes.Role) ?? throw new UnauthorizedException("Role is missing in token.");

        public string Username =>
            GetClaim(ClaimTypes.Name) ?? throw new UnauthorizedException("Username is missing in token.");

        private string GetClaim(string type)
        {
            return httpContextAccessor.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == type)?.Value;
        }
    }
}
