using FinanceTracker.Domain.Models.DTOs.TransactionDtos;

namespace FinanceTracker.Domain.Models.DTOs.ReportDtos
{
    public class ReportResultDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<TransactionDto> Transactions { get; set; }
    }
}
