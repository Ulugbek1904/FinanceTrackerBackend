using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class CreateAccountDto
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }
        public bool IsPrimary { get; set; }
    }
}
