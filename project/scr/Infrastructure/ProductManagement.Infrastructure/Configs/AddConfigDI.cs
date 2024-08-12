using Microsoft.Extensions.Configuration;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class AddConfigDI
    {
        public static IServiceCollection AddConfig(
             this IServiceCollection services, IConfiguration config)
        {

            //上学问模块配置
            services.Configure<PaidQAOption>(config.GetSection(PaidQAOption.PaidQA));
            //话题圈模块配置
            services.Configure<TopicOption>(config.GetSection(TopicOption.Topic));

            return services;
        }
    }
}
