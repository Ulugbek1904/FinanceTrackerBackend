using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string CategoryName { get; set; }
        public string AccountName { get; set; }
    }
}
