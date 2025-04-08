using FinanceTracker.Domain.Enums;
using System.Text.Json.Serialization;

namespace FinanceTracker.Domain.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public Guid UserId  { get; set; }
        public User User { get; set; }
        public AccountType Type { get; set; }
        public bool IsPrimary { get; set; }
        [JsonIgnore]
        public List<Transaction> Transactions { get; set; } = new();
    }
}
