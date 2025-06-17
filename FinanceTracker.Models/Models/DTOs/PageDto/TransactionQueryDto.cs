namespace FinanceTracker.Domain.Models.DTOs.PageDto
{
    public class TransactionQueryDto
    {
        public bool? IsIncome { get; set; }
        public int? CategoryId { get; set; }
        public Guid? AccountId { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
    }
}
