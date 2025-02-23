using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public Guid UserId  { get; set; }
        public TransactionSource Source { get; set; }
    }
}
