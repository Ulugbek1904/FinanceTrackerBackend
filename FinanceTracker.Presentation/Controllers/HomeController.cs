using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : RESTFulController
    {
        private readonly IStorageBroker storageBroker;

        public HomeController(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var users = this.storageBroker.SelectAll<User>();
            var result = string.Empty;
            foreach (var user in users)
            {
                result += user.LastName + user.FirstName + "\n";
            }

            return Ok(result);
        }
    }
}
