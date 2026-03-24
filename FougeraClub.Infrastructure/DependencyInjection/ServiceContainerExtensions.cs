using FougeraClub.Application.Interfaces.Services;
using FougeraClub.Infrastructure.Services.Otp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FougeraClub.Infrastructure.DependencyInjection
{
    public static class ServiceContainerExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ManagerOtpOptions>(configuration.GetSection("ManagerOtp"));
            services.AddScoped<IManagerOtpService, ManagerOtpService>();
            services.AddScoped<IOtpSender, NoopOtpSender>();

            return services;
        }
    }
}
