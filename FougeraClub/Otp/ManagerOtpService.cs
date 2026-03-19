using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FougeraClub.Web.Otp
{
    public class ManagerOtpService : IManagerOtpService
    {
        private const string SessionOtpCode = "ManagerOtpCode";
        private const string SessionOtpExpiresAt = "ManagerOtpExpiresAt";
        private const string SessionApprovedManagerName = "ManagerApprovedName";

        private readonly IOptions<ManagerOtpOptions> _options;
        private readonly IOtpSender _otpSender;

        public ManagerOtpService(IOptions<ManagerOtpOptions> options, IOtpSender otpSender)
        {
            _options = options;
            _otpSender = otpSender;
        }

        public async Task<OtpIssueResult> IssueOtpAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var options = _options.Value;
            var otp = options.UseFixedCode ? options.FixedCode : GenerateNumericOtp(options.CodeLength);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(Math.Max(1, options.ExpiryMinutes));

            httpContext.Session.SetString(SessionOtpCode, otp);
            httpContext.Session.SetString(SessionOtpExpiresAt, expiresAtUtc.ToString("O"));
            httpContext.Session.Remove(SessionApprovedManagerName);

            var deliveryEnabled = options.EnableDelivery && !string.IsNullOrWhiteSpace(options.Recipient);
            if (deliveryEnabled)
            {
                await _otpSender.SendAsync(options.Recipient, otp, cancellationToken);
            }

            return new OtpIssueResult
            {
                Success = true,
                DeliveryEnabled = deliveryEnabled,
                Message = deliveryEnabled
                    ? "تم إرسال OTP إلى المدير."
                    : "تم تجهيز OTP (وضع تجريبي بدون إرسال).",
                ExpiresAtUtcIso = expiresAtUtc.ToString("O")
            };
        }

        public Task<OtpVerifyResult> VerifyOtpAsync(HttpContext httpContext, string? otp, CancellationToken cancellationToken = default)
        {
            var inputOtp = (otp ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputOtp))
            {
                return Task.FromResult(new OtpVerifyResult { Success = false, Message = "ادخل OTP" });
            }

            var sessionOtp = httpContext.Session.GetString(SessionOtpCode);
            var sessionExpiry = httpContext.Session.GetString(SessionOtpExpiresAt);
            if (string.IsNullOrWhiteSpace(sessionOtp) || string.IsNullOrWhiteSpace(sessionExpiry))
            {
                return Task.FromResult(new OtpVerifyResult { Success = false, Message = "اطلب OTP أولاً" });
            }

            if (!DateTime.TryParse(sessionExpiry, out var expiresAtUtc) || DateTime.UtcNow > expiresAtUtc)
            {
                ClearOtpSession(httpContext);
                return Task.FromResult(new OtpVerifyResult { Success = false, Message = "OTP انتهت صلاحيته" });
            }

            if (!string.Equals(inputOtp, sessionOtp, StringComparison.Ordinal))
            {
                return Task.FromResult(new OtpVerifyResult { Success = false, Message = "OTP غير صحيح" });
            }

            var managerName = string.IsNullOrWhiteSpace(_options.Value.ManagerName) ? "ziad" : _options.Value.ManagerName.Trim();
            httpContext.Session.SetString(SessionApprovedManagerName, managerName);
            ClearOtpSession(httpContext);

            return Task.FromResult(new OtpVerifyResult
            {
                Success = true,
                Message = "تم التحقق بنجاح",
                ManagerName = managerName
            });
        }

        private static string GenerateNumericOtp(int length)
        {
            var otpLength = Math.Clamp(length, 4, 8);
            var buffer = new byte[otpLength];
            RandomNumberGenerator.Fill(buffer);

            var chars = new char[otpLength];
            for (int i = 0; i < otpLength; i++)
            {
                chars[i] = (char)('0' + (buffer[i] % 10));
            }

            return new string(chars);
        }

        private static void ClearOtpSession(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionOtpCode);
            httpContext.Session.Remove(SessionOtpExpiresAt);
        }
    }
}
