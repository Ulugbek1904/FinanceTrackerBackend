using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/password")]
    public class PasswordController : RESTFulController
    {
        private readonly IEmailService emailService;

        public PasswordController(IEmailService emailService)
        {
            this.emailService = emailService;
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
