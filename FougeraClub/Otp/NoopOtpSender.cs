using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FougeraClub.Web.Otp
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
