using FinanceTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace FinanceTracker.Domain.Models
{
    [DebuggerDisplay("FirstName nq")]
    public class User
    {
        public Guid Id { get; set; }
        [EmailAddress(ErrorMessage ="Email is not valid")]
        public string Email { get; set; }
        public string HashedPassword{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [EmailAddress]
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpExpiration { get; set; } = DateTime.UtcNow;
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; } = DateTime.UtcNow.AddDays(30);
        [JsonIgnore]
        public List<Account> Accounts { get; set; }
    }
}
