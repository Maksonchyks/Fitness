using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Identity.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationToken)
        {
            _logger.LogInformation("Email confirmation token for {Email}: {Token}", email, confirmationToken);

            await Task.CompletedTask;
        }

        public async Task SendPasswordResetAsync(string email, string resetToken)
        {
            _logger.LogInformation("Password reset token for {Email}: {Token}", email, resetToken);

            await Task.CompletedTask;
        }
    }
}
