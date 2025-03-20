
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FinanceTracker.Infrastructure.Brokers.Email
{
    public class EmailBroker : IEmailBroker
    {
        private readonly IConfiguration configuration;

        public EmailBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async ValueTask SendPasswordResetEmailAsync(string email, string PasswordResetOtp)
        {
            var apiKey = this.configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(this.configuration["SendGrid:FromEmail"], "Finance Tracker");
            var subject = "Password Reset";
            var to = new EmailAddress(email);
            var plainTextContent = "Here is your password reset OTP: " + PasswordResetOtp;
            var htmlContent = "<strong>Here is your password reset OTP: " + PasswordResetOtp + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }
}
