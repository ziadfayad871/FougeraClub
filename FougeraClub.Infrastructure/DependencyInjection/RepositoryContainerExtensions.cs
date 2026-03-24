using FougeraClub.Application.Interfaces.Repositories;
using FougeraClub.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FougeraClub.Infrastructure.DependencyInjection
{
    public static class RepositoryContainerExtensions
    {
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

            return services;
        }
    }
}
