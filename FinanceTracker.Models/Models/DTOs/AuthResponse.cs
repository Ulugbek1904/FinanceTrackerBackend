using FluentAssertions.Common;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiryDate { get; set; }
    }
}
