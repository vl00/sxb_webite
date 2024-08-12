using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace Sxb.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//使用AutoFac做IOC和AOP
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder//.UseKestrel().UseUrls("https://*:5001")
                .UseStartup<Startup>()
                .UseNLog();
             });
    }
}
