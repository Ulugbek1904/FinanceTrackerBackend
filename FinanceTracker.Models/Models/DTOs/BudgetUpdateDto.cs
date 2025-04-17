using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Domain.Models.DTOs
{
    public class BudgetUpdateDto
    {
        public string Name { get; set; }
        public decimal LimitAmount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int CategoryId { get; set; }
    }
}
