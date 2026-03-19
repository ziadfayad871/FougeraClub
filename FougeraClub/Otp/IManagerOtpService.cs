using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FougeraClub.Web.Otp
{
    public interface IManagerOtpService
    {
        Task<OtpIssueResult> IssueOtpAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
        Task<OtpVerifyResult> VerifyOtpAsync(HttpContext httpContext, string? otp, CancellationToken cancellationToken = default);
    }

    public class OtpIssueResult
    {
        public bool Success { get; set; }
        public bool DeliveryEnabled { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ExpiresAtUtcIso { get; set; }
    }

    public class OtpVerifyResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
    }
}
