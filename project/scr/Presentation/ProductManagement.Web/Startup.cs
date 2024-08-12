using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using PMS.CommentsManage.Repository;
using PMS.School.Repository;
using ProductManagement.Web.App_Start;
using ProductManagement.Web.Areas.PartTimeJob.Models.Common;
using ProductManagement.Web.Authentication;
using ProductManagement.Web.Middleware;
using ProductManagement.Framework.MSSQLAccessor;
using PMS.UserManage.Repository;
using ProductManagement.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Cache.Redis.Configuration;
using PMS.Infrastructure.Repository;
using ProductManagement.Framework.RabbitMQ;
using PMS.RabbitMQ.Handle;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace ProductManagement.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var fileBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            Configuration = fileBuilder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

            services.Configure<Setting>(Configuration.GetSection("Setting"));
            //EF
            services.AddDbContext<CommentsManageDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            //services.AddDbContext<SchoolManageDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("SchoolSqlServer")));

            //Dapper
            services.AddScopedMSSQLDbContext<UserDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("UserServer");
                c.Name = "UserServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

            services.AddScopedMSSQLDbContext<OperationDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("OperationPlateformConStr");
                c.Name = "OperationPlateform";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<ISchoolDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("ISchool");
                c.Name = "ISchool";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<ISchoolDataDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("SchoolSqlServer");
                c.Name = "SchoolSqlServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<JcDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("ISchool");
                c.Name = "Infrastructure";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });


            //RabbitMQ
            services.AddProductManagementRabbitMQ(option =>
            {
                var config = Configuration.GetSection("rabbitMQSetting").Get<RabbitMQOption>();
                option.AmqpUris = config.AmqpUris;
                option.Uri = config.Uri;
                option.ExtName = config.ExtName;
            }, new ProductManagement.Framework.Serialize.Json.NewtonsoftSerializer())
            .ScanMessage(typeof(SyncSchoolScoreHandler).Assembly, this.GetType().Assembly);


            //Redis
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MyRedisProfiler>();
            services.AddSingletonCacheRedis(config =>
            {
                var redisConfig = Configuration.GetSection("RedisConfig").Get<RedisConfig>();
                config.Database = redisConfig.Database;
                config.RedisConnect = redisConfig.RedisConnect;
                config.CloseRedis = redisConfig.CloseRedis;
                config.HaveLog = redisConfig.HaveLog;
            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.Cookie.Name = "ProductManagement";
                o.LoginPath = new PathString("/PartTimeJob/Home/Index");
                //o.AccessDeniedPath = new PathString("/PartTimeJob/Home");//没有权限跳转
            });
            //.AddOpenIdConnect(o => {
            //    o.ClientId = "server.hybrid";
            //    o.ClientSecret = "secret";
            //    o.Authority = "https://demo.identityserver.io/";
            //    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            //});

            //避免循环引用
            //services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRazorPages();
            services.AddControllersWithViews().AddControllersAsServices().AddNewtonsoftJson(
                options =>
                {
                    // 忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });


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
                builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            }
            //var applicationContainer = builder.Build();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                //loggerFactory.AddPGAccessorLog(LogLevel.Trace);
                //app.UseDeveloperExceptionPage();
                NLogBuilder.ConfigureNLog("nlog.Development.config");
            }
            else if (env.IsProduction())
            {
                NLogBuilder.ConfigureNLog("nlog.config");
                app.UseExceptionMiddleware();
            }
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            
            app.UseAuthentication();
            //配置默认路由
            //app.UseMvc(routes =>
            //{
            //    //默认路由
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{area=PartTimeJob}/{controller=Home}/{action=Index}/{id?}");

            //    //区域路由配置
            //    routes.MapRoute(
            //    name: "areas",
            //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                //默认路由
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=PartTimeJob}/{controller=Home}/{action=Index}/{id?}");
                //区域路由配置
                endpoints.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "PartTimeJob",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
