using Microsoft.Extensions.DependencyInjection;
using System;


namespace ProductManagement.Framework.SearchAccessor
{
    public static class AccessorServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedSearchAccessor(this IServiceCollection services, Action<SearchConfig> config) 
        {
            services.Configure(config);

            services.AddScoped<SearchConfigOptions>();

            services.AddScoped<ISearch, SearchContext>();

            return services;
        }
    }
}