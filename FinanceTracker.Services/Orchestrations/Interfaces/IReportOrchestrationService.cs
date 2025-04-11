using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface IReportOrchestrationService
    {
        ValueTask<FinancialReport> GenerateMonthlyReport(Guid userId, int year, int month);
    }
}
