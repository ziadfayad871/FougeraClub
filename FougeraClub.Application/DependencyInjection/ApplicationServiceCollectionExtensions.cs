using FougeraClub.Application.Interfaces.Services;
using FougeraClub.Application.Mappings;
using FougeraClub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FougeraClub.Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

            return services;
        }
    }
}
