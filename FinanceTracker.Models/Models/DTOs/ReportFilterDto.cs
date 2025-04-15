namespace FinanceTracker.Domain.Models.DTOs
{
    public class ReportFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? CategoryId { get; set; }
        public Guid? AccountId { get; set; }
    }
}
