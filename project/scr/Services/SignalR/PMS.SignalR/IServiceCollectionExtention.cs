using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PMS.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtention
    {
        public static void AddCustomSignalR(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, UserInfoProvider>();
        }
    }
}
