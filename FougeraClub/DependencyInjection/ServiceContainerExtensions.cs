using FougeraClub.Web.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace FougeraClub.Web.DependencyInjection
{
    public static class ServiceContainerExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<INotificationStore, InMemoryNotificationStore>();

            return services;
        }
    }
}
