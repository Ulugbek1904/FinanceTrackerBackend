namespace FinanceTracker.Domain.Models.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance { get; set; }
        public List<MonthlySummaryDto> MonthlySummary { get; set; }
        public List<AccountSummaryDto> AccountSummaries { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; }
        public List<CategorySummary> TopCategories { get; set; }
    }
}
