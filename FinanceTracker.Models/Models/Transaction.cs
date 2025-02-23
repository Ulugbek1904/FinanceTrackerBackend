using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public Category Category { get; set; }
        public TransactionSource Source { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}
