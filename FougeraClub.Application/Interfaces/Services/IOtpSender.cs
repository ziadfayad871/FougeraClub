namespace FougeraClub.Application.Interfaces.Services
{
    public interface IOtpSender
    {
        Task SendAsync(string recipient, string otp, CancellationToken cancellationToken = default);
    }
}
