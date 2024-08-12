using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using AspectCore.Extensions.Autofac;
using Autofac;
using ImportAndExport.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PMS.CommentsManage.Repository;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.Cache.Redis;
using PMS.School.Repository;

namespace ImportAndExport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            //Redis
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
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            services.AddDbContext<SchoolManageDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("SchoolSqlServer")));

            services.AddControllersWithViews();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //services.AddControllersWithViews();

            #region autofac×¢Èë
            //var builder = new ContainerBuilder();
            //builder.Populate(services);

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
            builder.RegisterDynamicProxy();
            //return new AutofacServiceProvider(applicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
