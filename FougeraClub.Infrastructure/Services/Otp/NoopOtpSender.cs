using FougeraClub.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FougeraClub.Infrastructure.Services.Otp
{
    public class NoopOtpSender : IOtpSender
    {
        private readonly ILogger<NoopOtpSender> _logger;

        public NoopOtpSender(ILogger<NoopOtpSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string recipient, string otp, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("OTP delivery skipped. Recipient: {Recipient}", recipient);
            return Task.CompletedTask;
        }
    }
}
