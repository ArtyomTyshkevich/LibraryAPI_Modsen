using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Setup
{
    public static class CacheSetup
    {
        public static void ConfigureCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(option =>
                option.Configuration = configuration.GetConnectionString("Cache"));
        }
    }
}