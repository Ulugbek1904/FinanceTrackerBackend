using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Models
{
    public class RecurringTransaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } = DateTime.UtcNow;

        public RecurringType RecurrenceType { get; set; }

        public bool IsIncome { get; set; }
        public DateTime? LastProcessedDate { get; set; } = DateTime.UtcNow;
    }
}
