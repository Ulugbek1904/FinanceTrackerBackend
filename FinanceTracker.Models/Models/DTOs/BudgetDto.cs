namespace FinanceTracker.Domain.Models.DTOs
{
    public class BudgetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal LimitAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
