using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace ProductManagement.Framework.MongoDb
{
    public static class MongoDBAccessorExtensions
    {
        public static IServiceCollection AddMongoDbAccessor(this IServiceCollection services, Action<MongoDbConfig> config)
        {
            services.Configure(config);

            services.AddScoped<MongoConfigOptions>();

            services.AddScoped<IMongoService, MongoDbContext>();

            return services;
        }

        public static IServiceCollection AddMongoDbAccessor<T>(this IServiceCollection services, Action<MongoDbConfig<T>> config)
             where T : IMongoProvider
        {
            services.Configure(config);

            services.AddScoped<MongoConfigOptions<T>>();

            services.AddScoped<IMongoService<T>, MongoDbContext<T>>();

            return services;
        }
    }
}
