using System.Threading;
using System.Threading.Tasks;

namespace FougeraClub.Web.Otp
{
    public interface IOtpSender
    {
        Task SendAsync(string recipient, string otp, CancellationToken cancellationToken = default);
    }
}
