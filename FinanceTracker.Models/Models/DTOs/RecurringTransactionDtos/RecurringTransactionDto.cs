using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models.DTOs.RecurringTransactionDtos
{
    public class RecurringTransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public RecurringType Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CategoryName { get; set; }
        public string AccountName { get; set; }
    }
}
