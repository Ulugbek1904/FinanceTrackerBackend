using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class TransactionCreateDto
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public int? CategoryId { get; set; }
        public Guid AccountId { get; set; }
    }
}
