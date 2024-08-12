using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.HttpDelegatingHandlers;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace ProductManagement.API.Http
{
    public static class IServiceCollectionExtension
    {
        public static void AddProductManagementAPIHttps(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<HttpLogHandler>();

            services.Configure<OrgClientConfig>(config.GetSection(nameof(OrgClientConfig)));

            services.AddHttpClient<IOrgServiceClient, OrgServiceClient>()
                 .AddHttpMessageHandler<HttpLogHandler>()
                 .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(3))
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }))
                 .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))); //添加熔断策略，配合上面重试机制连续5遍后，打断后面请求操作，一个周期30秒

            services.AddHttpClient<IGoodsServiceClient, GoodsServiceClient>()
                 .AddHttpMessageHandler<HttpLogHandler>()
                 .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(30))
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }))
                 .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))); //添加熔断策略，配合上面重试机制连续5遍后，打断后面请求操作，一个周期30秒

            services.Configure<WXWorkClientConfig>(config.GetSection(nameof(WXWorkClientConfig)));
            services.AddHttpClient<IWXWorkServiceClient, WXWorkServiceClient>()
                 .AddHttpMessageHandler<HttpLogHandler>()
                 .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(30))
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }))
                 .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))); //添加熔断策略，配合上面重试机制连续5遍后，打断后面请求操作，一个周期30秒

            //推荐接口
            services.Configure<RecommendClientConfig>(config.GetSection(nameof(RecommendClientConfig)));
            AddClient<IRecommendClient, RecommendClient>(services);



            services.Configure<MarketingClientConfig>(config.GetSection(nameof(MarketingClientConfig)))
                .AddHttpClient<IMarketingServiceClient, MarketingServiceClient>();



            //UV统计接口
            services.Configure<StaticInsideConfig>(config.GetSection(nameof(StaticInsideConfig)));
            AddClient<IStaticInsideClient, StaticInsideClient>(services);
        }

        internal static void AddClient<T, TImpl>(IServiceCollection services)
            where T : class
            where TImpl : class, T
        {
            services.AddHttpClient<T, TImpl>()
                 .AddHttpMessageHandler<HttpLogHandler>()
                 .AddPolicyHandler(request => Policy.TimeoutAsync<HttpResponseMessage>(5))
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }))
                 .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))); //添加熔断策略，配合上面重试机制连续5遍后，打断后面请求操作，一个周期30秒
        }

    }
}
