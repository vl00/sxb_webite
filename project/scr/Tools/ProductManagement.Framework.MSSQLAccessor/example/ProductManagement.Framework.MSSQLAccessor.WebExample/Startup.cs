using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Service;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Data;
using NLog.Web;
using NLog.Extensions.Logging;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using Microsoft.AspNetCore.Mvc;
using App.Metrics;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("pgConnectionSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appMetrics = Configuration.GetSection("AppMetrics");
            var aspNetMetrics = Configuration.GetSection("AspNetMetrics");

            //APPMetrics
            if (Environment.IsDevelopment())
            {
                //services.AddMetrics(appMetrics,options =>
                //{
                //    //options.GlobalTags.Add("app", "HM_PGAccessor");
                //    //options.GlobalTags.Add("env", Environment.EnvironmentName);
                //})
                //.AddHealthChecks()
                //.AddJsonSerialization()
                //.AddReporting(factory =>
                // {
                //     var database = Configuration.GetSection("APPMetricsConfig:InfluxDBSettings:Database").Get<string>();
                //     var uri = new Uri(Configuration.GetSection("APPMetricsConfig:InfluxDBSettings:BaseAddress").Get<string>());
                //     var reportInterval= Configuration.GetSection("APPMetricsConfig:InfluxDBSettings:ReportInterval").Get<double>();

                //     factory.AddInfluxDb(new InfluxDBReporterSettings
                //     {
                //         InfluxDbSettings = new InfluxDBSettings(database, uri),
                //         ReportInterval = TimeSpan.FromSeconds(reportInterval)
                //     });
                // })
                //.AddMetricsMiddleware(aspNetMetrics);
            }

            //PG
            services.AddScopedMSSQLDbContext(c =>
            {
                var PGconfig = Configuration.GetSection("DbConnectionStrings").Get<List<ConnectionConfig>>().FirstOrDefault();
                c.Name = PGconfig.Name;
                c.LoadBalancer = PGconfig.LoadBalancer;
                c.RealDbConnectionStrings = PGconfig.RealDbConnectionStrings;
            });

            services.AddScoped<GroupOrderProgressRepository>();
            services.AddScoped<OrderStatusLogRepository>();
            services.AddScoped<GroupOrderProgressService>();
            services.AddScoped<OrderStatusLogService>();

            services.AddMvc(options => options.AddMetricsResourceFilter());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            //APPMetrics
            app.UseMetrics();
            app.UseMetricsReporting(lifetime);

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            //NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Trace, true, true);
            //NpgsqlLogManager.Provider = new LoggingProvider(loggerFactory,NpgsqlLogLevel.Trace);

            loggerFactory.AddNLog();
            //loggerFactory.AddPGAccessorLog(LogLevel.Trace);

            app.AddNLogWeb();

            env.ConfigureNLog("nlog.config");

            app.UseMvc();
        }
    }
}
