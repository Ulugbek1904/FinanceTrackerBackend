using FinanceTracker.Domain.Models.DTOs.ReportDtos;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface IReportOrchestrationService
    {
        ValueTask<FinancialReport> GenerateMonthlyReport(Guid userId, int year, int month);
        ValueTask<ReportResultDto> GetUserReportDataByPeriod(Guid userId, ReportFilterDto filter);
    }
}
