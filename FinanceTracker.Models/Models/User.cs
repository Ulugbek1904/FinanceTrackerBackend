using FinanceTracker.Domain.Enums;
using System.Text.Json.Serialization;

namespace FinanceTracker.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }
    }
}
