using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : RESTFulController
    {
        private readonly IDashboardProcessingService processingService;
        private readonly IStorageBroker storageBroker;
        private readonly StorageBroker storageBroker1;

        public HomeController(
            IDashboardProcessingService processingService,
            IStorageBroker storageBroker,
            StorageBroker storageBroker1)
        {
            this.processingService = processingService;
            this.storageBroker = storageBroker;
            this.storageBroker1 = storageBroker1;
        }
        [HttpGet]
        public IActionResult GetDashboardData()
        {
            var user = this.storageBroker.SaveChangesAsync();

            return Ok();
        }
    }
}
