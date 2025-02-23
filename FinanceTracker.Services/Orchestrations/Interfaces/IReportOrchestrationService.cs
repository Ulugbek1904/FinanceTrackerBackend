using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface IReportOrchestrationService
    {
        FinancialReport GenerateMonthlyReport(Guid userId, int year, int month);
    }
}
