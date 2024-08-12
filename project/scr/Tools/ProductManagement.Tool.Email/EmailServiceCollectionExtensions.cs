using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ProductManagement.Tool.Email
{
    public static class EmailServiceCollectionExtensions
    {

        public static IServiceCollection AddScopedMailNotification(this IServiceCollection services, Action<MailNotificationConfiguration> config)
        {
            services.Configure(config);
            services.AddScoped<MailNotificationConfiguration>();

            services.AddScoped<IEmailClient, EmailClient>();

            return services;
        }
    }
}
