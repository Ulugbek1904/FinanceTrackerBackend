using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        ValueTask RevokeAsync();
    }
}
