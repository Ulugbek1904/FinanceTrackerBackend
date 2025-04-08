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
        [EmailAddress]
        public string Email { get; set; }
        public string Password{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        [EmailAddress]
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpExpiration { get; set; } 
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        [JsonIgnore]
        public List<Account> Accounts { get; set; }
    }
}
