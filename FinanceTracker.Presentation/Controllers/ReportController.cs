using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportController : RESTFulController
    {
        private readonly IReportOrchestrationService reportOrchestration;

        public ReportController(IReportOrchestrationService reportOrchestration)
        {
            this.reportOrchestration = reportOrchestration;
        }

        [HttpGet("monthly/{year}/{month}")]
        [Authorize]
        public async ValueTask<IActionResult> GetMonthlyReport(int year, int month)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized();

            var report = await reportOrchestration
                .GenerateMonthlyReport(userId.Value, year, month);

            return Ok(report);
        }

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return null;
            if (Guid.TryParse(userIdClaim.Value, out Guid userId))
                return userId;
            return null;
        }
    }
}
