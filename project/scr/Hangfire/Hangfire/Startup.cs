using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire.Console;
using Hangfire.ConsoleWeb;
using Hangfire.ConsoleWeb.Jobs;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using PMS.CommentsManage.Repository;
using PMS.School.Repository;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Serialize.Json;
using Sxb.Web.Utils;
using ProductManagement.Framework.MSSQLAccessor;
using Microsoft.Extensions.Hosting;

namespace Hangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var redisConfig = Configuration.GetSection("RedisConfig").Get<RedisConfig>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MyRedisProfiler>();
            services.AddSingletonCacheRedis(config =>
            {
                config.Database = redisConfig.Database;
                config.RedisConnect = redisConfig.RedisConnect;
                config.CloseRedis = redisConfig.CloseRedis;
                config.HaveLog = redisConfig.HaveLog;
            }, new ProductManagement.Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            }));

            services.AddDbContext<CommentsManageDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ProductServer")));

            //services.AddDbContext<SchoolManageDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("SchoolSqlServer")));


            services.AddScopedMSSQLDbContext<PMS.OperationPlateform.Domain.DBContext.OperationDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("OperationServer");
                c.Name = "OperationPlateform";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<PMS.OperationPlateform.Domain.DBContext.ISchoolDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("Infrastructure");
                c.Name = "ISchool";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<PMS.OperationPlateform.Domain.DBContext.ISchoolDataDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("SchoolData");
                c.Name = "SchoolData";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

            //RabbitMQ
            services.AddProductManagementRabbitMQ(option =>
            {
                var config = Configuration.GetSection("rabbitMQSetting").Get<RabbitMQOption>();
                option.AmqpUris = config.AmqpUris;
                option.Uri = config.Uri;
            }, new NewtonsoftSerializer())
            .ScanMessage(this.GetType().Assembly);


            services.AddLogging();
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"));
                config.UseConsole();

                //using json config file to build RecurringJob automatically.
                config.UseRecurringJob("recurringJob.json",true);
                //using RecurringJobAttribute to build RecurringJob automatically.
                //config.UseRecurringJob(typeof(RecurringJobService));
            }
            );
            services.AddHangfireServer();

            services.AddRazorPages();
            services.AddControllersWithViews().AddControllersAsServices().AddNewtonsoftJson();




            //return new AutofacServiceProvider(applicationContainer);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region autofac注入
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            var list = new List<Assembly>();
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                if (lib.Name.Contains("ProductManagement.Framework.SearchAccessor"))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                }
                //else if (lib.Name.Contains("ProductManagement.Framework."))
                //{
                //    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().SingleInstance();
                //}
                //else if (lib.Name.Contains("PMS.CommentsManage.Repository"))
                //{
                //    builder.RegisterAssemblyTypes(assembly).InstancePerRequest();
                //}
                else
                {
                    //var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                    if (lib.Name.StartsWith("PMS."))
                    {
                        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().InstancePerLifetimeScope();
                    }
                    else
                    {
                        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
                    }
                }

                //var types = assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(DbContext)));
                //if(types != null)
                //{
                //    foreach (var item in types)
                //    {
                //        builder.RegisterType(item).AsSelf().InstancePerLifetimeScope();
                //    }
                //}
            }
            //builder.Populate(services);
            //var applicationContainer = builder.Build();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseWelcomePage("/hangfire");

            //根据请求选择语言
            var supportedCultures = new[]
                {
                    new CultureInfo("zh-CN")
                    //new CultureInfo("en-US")
                 };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-CN"),

                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures,

                RequestCultureProviders = new List<IRequestCultureProvider>
                                    {
                                        new QueryStringRequestCultureProvider(),
                                        new CookieRequestCultureProvider(),
                                        new AcceptLanguageHeaderRequestCultureProvider()
                                     }
            });

            //GlobalConfiguration.Configuration
            //                   .UseActivator(new HangfireActivator(serviceProvider));

            //Hangfire
            app.UseHangfireServer();
            app.UseHangfireDashboard("", new DashboardOptions
            {
                //Authorization = new[]
                //{
                //    new BasicAuthAuthorizationFilter(
                //        new BasicAuthAuthorizationFilterOptions
                //        {
                //          RequireSsl = false,
                //          SslRedirect = false,
                //          LoginCaseSensitive = true,
                //          Users = new []
                //                {
                //                new BasicAuthAuthorizationUser
                //                    {
                //                    Login = "admin",
                //                    PasswordClear =  "ABC123.."
                //                    }
                //                }
                //        })
                //}
            });

            app.UseHangfireDashboard();

            //添加周期性任务
            //RecurringJob.AddOrUpdate<StatisticalSchoolScoreJob>("统计学校分数", s => s.Excute(null), "0 */1 * * *", TimeZoneInfo.Local);

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
