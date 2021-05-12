using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNetQ.RabbitMQ.Domain.Support.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}