namespace FinanceTracker.Domain.Models.DTOs.BudgetDtos
{
    public class BudgetStatsDto
    {
        public decimal TotalSpent { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal LimitAmount { get; set; }
    }
}
