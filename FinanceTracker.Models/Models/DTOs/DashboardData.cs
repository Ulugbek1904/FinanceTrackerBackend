namespace FinanceTracker.Domain.Models.DTOs
{
    public class DashboardData
    {
        public decimal CurrentBalance { get; set; }
        public Dictionary<string, decimal> MonthlyTrend { get; set; }
    }
}
