namespace FinanceTracker.Domain.Models
{
    public class FinancialReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance { get; set; }
        public List<CategorySummary> Categories { get; set; }
    }

    public class CategorySummary
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
