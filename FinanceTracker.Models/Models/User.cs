using FinanceTracker.Domain.Enums;
using System.Text.Json.Serialization;

namespace FinanceTracker.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpExpiration { get; set; } 
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }

        public static object FindFirst(string nameIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
