using FinanceTracker.Domain.Models.DTOs;
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
    [Route("api/auth")]
    public class AuthController : RESTFulController
    {
        private readonly IAuthService authService;
        private readonly IAuthProvider provider;
        private readonly IUserService userService;
        private readonly IEmailService emailService;

        public AuthController(
            IAuthService authService,
            IAuthProvider provider,
            IUserService userService,
            IEmailService emailService)
        {
            this.authService = authService;
            this.provider = provider;
            this.userService = userService;
            this.emailService = emailService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await this.authService.LoginAsync(request.Email, request.Password);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var response = await this.authService.
                RefreshTokenAsync(refreshToken);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async ValueTask<IActionResult> Logout()
        {
            await this.authService.RevokeAsync();
            return NoContent();
        }

        [HttpGet("get-me")]
        [Authorize]
        public async ValueTask<IActionResult> GetMe(string accessToken)
        {
            var principal = this.provider.ValidateToken(accessToken);

            if(principal == null)
                throw new SecurityTokenException("Invalid token.");

            Guid userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == Guid.Empty)
                throw new SecurityTokenException("Invalid token.");

            var user = await this.userService.RetrieveUserByIdAsync(userId);

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
                user.Password
            };

            return Ok(result);
        }


        [HttpPost("forgot-password")]
        public async ValueTask<IActionResult> ForgotPassword(string email)
        {
            await this.emailService.SendPasswordResetEmailAsync(email);

            return Ok("Reset code sent to email.");
        }

        [HttpPost("reset-password")]
        public async ValueTask<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await this.emailService.ResetPasswordAsync(request.Email, request.OtpCode, request.NewPassword);

            return Ok("Password reset successfully.");
        }
    }
}
