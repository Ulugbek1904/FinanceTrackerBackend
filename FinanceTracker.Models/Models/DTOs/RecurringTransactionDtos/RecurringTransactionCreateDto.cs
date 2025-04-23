using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models.DTOs.RecurringTransactionDtos
{
    public class RecurringTransactionCreateDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public RecurringType Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CategoryId { get; set; }
        public Guid AccountId { get; set; }
    }
}
