using Microsoft.Extensions.DependencyInjection;
using System;


namespace ProductManagement.Framework.MSSQLAccessor
{
    public static class AccessorServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedMSSQLDbContext<T>(this IServiceCollection services, Action<ConnectionConfig<T>> config) where T :class
        {
            services.Configure(config);

            services.AddScoped<AccessorConfigOptions<T>>();

            services.AddScoped<ConnectionsManager<T>>();

            services.AddScoped<T>();

            return services;
        }

    }
}
