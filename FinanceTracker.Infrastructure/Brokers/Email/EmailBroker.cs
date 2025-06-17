using System.Net.Mail;
using System.Net;

namespace FinanceTracker.Infrastructure.Brokers.Email
{
    public class EmailBroker : IEmailBroker
    {

        public async ValueTask SendPasswordResetEmailAsync(string email, string PasswordResetOtp)
        {
            var message = new MailMessage
            {
                From = new MailAddress("julugbek023@gmail.com", "Finance Tracker"),
                Subject = "Password Reset",
                Body = "<strong>Here is your password reset OTP: " + PasswordResetOtp + "</strong>",
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(email));

            using (var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("julugbek023@gmail.com", "waggutwspfohpjao")
            })
            {
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
