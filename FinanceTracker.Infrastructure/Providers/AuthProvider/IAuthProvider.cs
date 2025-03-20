using FinanceTracker.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FinanceTracker.Infrastructure.Providers.AuthProvider
{
    public interface IAuthProvider
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        public ClaimsPrincipal ValidateToken(string token);
        TokenValidationParameters GetValidationParameters();
    }
}
