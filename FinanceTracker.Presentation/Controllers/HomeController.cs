using FinanceTracker.Infrastructure.Providers.AuthProvider;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : RESTFulController
    {
        private readonly IAuthProvider provider;
        private readonly IUserService userService;

        public HomeController(
            IAuthProvider provider,
            IUserService userService)
        {
            this.provider = provider;
            this.userService = userService;
        }

        [HttpGet("get-me")]
        [Authorize]
        public async ValueTask<IActionResult> GetMe(string accessToken)
        {
            var principal = this.provider.ValidateToken(accessToken);

            if (principal == null)
                throw new SecurityTokenException("Invalid token.");

            Guid userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == Guid.Empty)
                throw new SecurityTokenException("Invalid token.");

            var user = await this.userService.RetrieveUserByIdAsync(userId);

            var profilePictureUrl = user.ProfilePictureUrl; 

            var result = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.CreatedBy,
                user.CreatedAt,
                user.UpdatedAt,
                user.IsActive,
                user.Role,
                ProfilePictureUrl = profilePictureUrl
            };

            return Ok(result);
        }


    }
}
