using FinanceTracker.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : RESTFulController
    {
        private readonly IDashboardProcessingService processingService;

        public DashboardController(IDashboardProcessingService processingService)
        {
            this.processingService = processingService;
        }

        [HttpGet("summary")]
        public async ValueTask<IActionResult> GetDashboardSummary()
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized();

            var dashboardData = await processingService.GetDashboardData(userId.Value);
            return Ok(dashboardData);
        }

        private Guid? GetUserId()
        {
            var userClaims = User.FindFirst(ClaimTypes.NameIdentifier);

            if(userClaims is null)
                return null;

            if (Guid.TryParse(userClaims.Value, out Guid userId))
                return userId;

            return null;
        }
    }
}
