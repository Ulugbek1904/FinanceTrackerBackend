namespace FinanceTracker.Domain.Models.DTOs
{
    public class MonthlySummaryDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
