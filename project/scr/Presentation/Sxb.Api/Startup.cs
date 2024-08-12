using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using Sxb.Api.Response;
using Sxb.Api.Middleware;
using NLog.Extensions.Logging;
using Autofac;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Runtime.Loader;
using Autofac.Extensions.DependencyInjection;
using PMS.CommentsManage.Repository;
using Microsoft.EntityFrameworkCore;
using PMS.Infrastructure.Repository;
using PMS.UserManage.Repository;
using PMS.School.Repository;
using Microsoft.AspNetCore.Http;
using ProductManagement.Framework.Cache.RedisProfiler;
using Sxb.Api.Utils;
using Newtonsoft.Json;
using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Cache.Redis.Configuration;
using AutoMapper;
using Sxb.Api.Map;
using NLog.Web;
using Polly;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using iSchool.Internal.API.UserModule;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace Sxb.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var fileBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            Configuration = fileBuilder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private MapperConfiguration _mapperConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new VoMapperConfiguration(Configuration.GetSection("ImageSetting")["QueryImager"]));
            });
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());

            //EF
            services.AddDbContext<CommentsManageDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            //services.AddDbContext<SchoolManageDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("SchoolSqlServer")));

            //Dapper
            services.AddScopedMSSQLDbContext<UserDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("UserServer");
                c.Name = "ProductManagement";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

            //services.AddScopedMSSQLDbContext<OperationDBContext>(c =>
            //{
            //    var SQLConfig = Configuration.GetConnectionString("OperationPlateformConStr");
            //    c.Name = "OperationPlateform";
            //    c.LoadBalancer = SQLConfig;
            //    c.RealDbConnectionStrings = new List<string> { SQLConfig };
            //});
            //services.AddScopedMSSQLDbContext<OperationQueryDBContext>(c =>
            //{
            //    var SQLConfig = Configuration.GetConnectionString("OperationPlateformConStr");
            //    c.Name = "OperationPlateform";
            //    c.LoadBalancer = SQLConfig;
            //    c.RealDbConnectionStrings = new List<string> { SQLConfig };
            //});
            //services.AddScopedMSSQLDbContext<OperationCommandDBContext>(c =>
            //{
            //    var SQLConfig = Configuration.GetConnectionString("OperationPlateformConStr");
            //    c.Name = "OperationPlateform";
            //    c.LoadBalancer = SQLConfig;
            //    c.RealDbConnectionStrings = new List<string> { SQLConfig };
            //});
            services.AddScopedMSSQLDbContext<OperationDBContext>(c =>
            {
                var config = Configuration.GetSection("ConnectionStrings:OperationServer").Get<CQRSConfig>();
                c.Config("OperationPlateform", config);
            });
            services.AddScopedMSSQLDbContext<OperationQueryDBContext>(c =>
            {
                var config = Configuration.GetSection("ConnectionStrings:OperationServer").Get<CQRSConfig>();
                c.Config("OperationPlateform", config);
            });
            services.AddScopedMSSQLDbContext<OperationCommandDBContext>(c =>
            {
                var config = Configuration.GetSection("ConnectionStrings:OperationServer").Get<CQRSConfig>();
                c.Config("OperationPlateform", config);
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
                var SQLConfig = Configuration.GetConnectionString("iSchoolServer");
                c.Name = "Infrastructure";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

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
            }, new ProductManagement.Framework.Cache.Redis.Serializer.NewtonsoftSerializer(new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
                TypeNameHandling = TypeNameHandling.All
            }));

            services.AddHttpClient<iSchool.Data.API.ISchoolDataClient>(c =>
            {
                string iSchoolDataBaseAddress = this.Configuration.GetSection("ExternalInterface").GetValue<string>("iSchoolDataBaseAddress");
                c.BaseAddress = new Uri(iSchoolDataBaseAddress);
            })
           .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Add User API Type HttpClient
            services.AddHttpClient<UserApiServices>(c =>
            {
                string externalUrl = this.Configuration.GetSection("ExternalInterface").GetValue<string>("UserBaseAddress");
                c.BaseAddress = new Uri(externalUrl);
            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sxb API", Version = "v1", Description = "API文档", });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, Array.Empty<string>()
                    }
                });//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });

                //添加读取注释服务
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Sxb.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });

            //Model验证响应逻辑
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Name = e.Key,
                            Message = e.Value.Errors.First().ErrorMessage
                        }
                        ).ToArray();

                    //return new BadRequestObjectResult(ResponseResult.Failed(errors.FirstOrDefault().Message));
                    //不返回400
                    return new ObjectResult(ResponseResult.Failed(errors.FirstOrDefault().Message));
                };
            });

            //跨域设置
            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                //测试先开放全部源，上线读配置文件开放的域名
                policy.SetIsOriginAllowed(origin => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
                });

            });


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddControllers().AddControllersAsServices().AddNewtonsoftJson();
            //#region autofac注入

            //var builder = new ContainerBuilder();
            //builder.Populate(services);

            //var deps = DependencyContext.Default;
            //var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            //var list = new List<Assembly>();
            //foreach (var lib in libs)
            //{
            //    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
            //    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            //}
            //var applicationContainer = builder.Build();

            //#endregion autofac注入

            //return new AutofacServiceProvider(applicationContainer);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            var list = new List<Assembly>();
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                if (lib.Name.Contains("PMS."))
                {
                    //var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();//.AsSelf()
                                                                                      //.OnActivated(e => Console.WriteLine(lib.Name + " OnActivated创建之后调用!"))
                                                                                      //.OnRelease(e => Console.WriteLine(lib.Name + " OnRelease在释放占用的资源之前调用!"));
                }
            }
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                app.UseExceptionMiddleware();
                NLogBuilder.ConfigureNLog("nlog.config");
            }

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            app.UseStatusCodeMiddleware();
            app.UseCors("LimitRequests");
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            app.UseAuthentication();

            app.UseHttpsRedirection();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "/swagger/index.html");
            //});

            app.UseRouting();
            //全局跨域
            app.UseCors("any");
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}