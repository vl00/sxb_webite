using Autofac;
using AutoMapper;
using CommentApp.App_Start;
using iSchool.Internal.API.OperationModule;
using iSchool.Internal.API.RankModule;
using iSchool.Internal.API.UserModule;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog.Web;
using PMS.CommentsManage.Repository;
using PMS.Infrastructure.Repository;
using PMS.School.Infrastructure;
using PMS.Search.Elasticsearch;
using PMS.UserManage.Repository;
using Polly;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Aliyun.Common;
using ProductManagement.API.Http;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.SearchAccessor;
using ProductManagement.Tool.Amap;
using ProductManagement.Tool.Amap.Common;
using Sxb.GenerateNo;
using Sxb.PCWeb.Middleware;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using ProductManagement.Framework.MongoDb;
using PMS.OperationPlateform.Repository;

namespace CommentApp
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
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.Configure<ImageSetting>(Configuration.GetSection("ImageSetting"));

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new VoMapperConfiguration());
            });
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());

            services.Configure<AliTextConfig>(Configuration.GetSection("AliTextConfig"));
            services.AddHttpClient<IText, Text>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //EF
            services.AddDbContext<CommentsManageDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ProductServer")));

            //services.AddDbContext<SchoolManageDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("SchoolData")));

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
                var SQLConfig = Configuration.GetConnectionString("Infrastructure");
                c.Name = "ISchool";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<ISchoolDataDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("SchoolData");
                c.Name = "SchoolData";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<JcDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("Infrastructure");
                c.Name = "Infrastructure";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<CityDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("SchoolData");
                c.Name = "SchoolData";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<TopicCircleDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("TopicCircleServer");
                c.Name = "TopicCircleServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

            services.AddScopedMSSQLDbContext<PaidQADBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("PaidQAServer");
                c.Name = "PaidQAServer";
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
            }, new ProductManagement.Framework.Serialize.Json.NewtonsoftSerializer());
            //.ScanMessage(typeof(SyncSchoolScoreHandler).Assembly, this.GetType().Assembly);

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
                ContractResolver = new PrivateSetterContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new SchFType0JsonConverter()
                }
            }));

            //Mongo
            services.AddMongoDbAccessor(config =>
            {
                var dbs = Configuration.GetSection("MongoConfig").Get<List<MongoDbConfig>>();
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });

            //ES
            services.Configure<EsConfig>(Configuration.GetSection("SearchConfig"));
            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = Configuration.GetSection("SearchConfig").Get<ProductManagement.Framework.SearchAccessor.SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });

            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";

            //远程用户中心
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.IsAjax())
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                        var currentUrl = new UriBuilder(context.RedirectUri);
                        var returnUrl = new UriBuilder
                        {
                            Scheme = currentUrl.Scheme,
                            Host = currentUrl.Host,
                            Port = currentUrl.Port,
                            Path = context.Request.Path,
                            Query = string.Join("&", context.Request.Query.Select(q => q.Key + "=" + q.Value))
                        };
                        var userHost = Configuration.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");
                        var redirectUrl = new UriBuilder
                        {
                            Scheme = "https",
                            Host = (userHost??"").Replace("https://",""),
                            //Port = 80,
                            Path = new PathString("/login/login-pc.html"),
                            Query = QueryString.Create("returnUrl", returnUrl.Uri.ToString()).Value
                        };
                        context.Response.Redirect(redirectUrl.Uri.ToString());
                        return Task.CompletedTask;
                    }
                };
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "iSchoolAuth";
                options.Cookie.Domain = ".sxkid.com";
                options.Cookie.Path = "/";
                //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"c:\shared-auth-ticket-keys\"));
                options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
            });

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //HTTP 请求

            services.Configure<AmapConfig>(Configuration.GetSection("AmapConfig"));
            services.AddHttpClient<IAmapClient, AmapClient>()
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.Configure<ProductManagement.API.Http.Common.SchoolSearch>(Configuration.GetSection("SchoolNameSearch"));
            services.AddHttpClient<ISearchClient, SearchClient>()
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));
            //用户中心请求地址注入
            services.Configure<ProductManagement.API.Http.Common.UserSystemConfig>(Configuration.GetSection("UserSystemConfig"));
            services.AddHttpClient<IUserServiceClient, UserServiceClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            //直播接口
            services.Configure<LiveConfig>(Configuration.GetSection("LiveConfig"));
            services.AddHttpClient<ILiveServiceClient, LiveServiceClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.AddHttpClient<iSchool.Data.API.ISchoolDataClient>(c =>
            {
                string iSchoolDataBaseAddress = this.Configuration.GetSection("ExternalInterface").GetValue<string>("iSchoolDataBaseAddress");
                c.BaseAddress = new Uri(iSchoolDataBaseAddress);
            })
           .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //微信公众号服务中心接口
            services.Configure<WeChatAppConfig>(Configuration.GetSection("WeChatAppConfig"));
            services.AddHttpClient<IWeChatAppClient, WeChatAppClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            //上学帮支付服务中心接口
            services.Configure<FinanceCenterConfig>(Configuration.GetSection("FinanceCenterConfig"));
            services.AddHttpClient<IFinanceCenterServiceClient, FinanceCenterServiceClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            //运营后台接口
            services.Configure<OperationConfig>(Configuration.GetSection("OperationConfig"));
            services.AddHttpClient<IOperationClient, OperationClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            //Add OperationPlateform API Type HttpClient
            services.AddHttpClient<OperationApiServices>(c =>
           {
               string externalUrl = this.Configuration.GetSection("ExternalInterface").GetValue<string>("OperationPlateFormBaseAddress");
               c.BaseAddress = new Uri(externalUrl);
           })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Add User API Type HttpClient
            services.AddHttpClient<UserApiServices>(c =>
           {
               string externalUrl = this.Configuration.GetSection("ExternalInterface").GetValue<string>("UserBaseAddress");
               c.BaseAddress = new Uri(externalUrl);
           })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Add Rank API Type HttpClient
            services.AddHttpClient<RankApiServices>(c =>
            {
                string externalUrl = this.Configuration.GetSection("ExternalInterface").GetValue<string>("RankAddress");
                c.BaseAddress = new Uri(externalUrl);
            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //运营后台接口
            services.Configure<OperationConfig>(Configuration.GetSection("OperationConfig"));
            services.AddHttpClient<IOperationClient, OperationClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            //微信公众号服务中心接口
            services.Configure<WeChatAppConfig>(Configuration.GetSection("WeChatAppConfig"));
            services.AddHttpClient<IWeChatAppClient, WeChatAppClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));
            services.AddEasyWeChat();

            //依赖注入httpClient
            services.AddHttpClient();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            //services.AddMvc(option =>
            //{
            //    //option.EnableEndpointRouting = false;
            //    option.Filters.Add(typeof(CityChangeFilterAttribute));
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages().AddRazorRuntimeCompilation();
            //services.AddControllersWithViews(option => { option.Filters.Add(typeof(CityChangeFilterAttribute)); }).AddControllersAsServices().AddNewtonsoftJson();
            services.AddControllersWithViews().AddControllersAsServices().AddNewtonsoftJson();

            //services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();

            services.AddAutoMapper(typeof(Startup));

            services.AddMediatR(typeof(Startup));

            services.AddSingleton<ISxbGenerateNo, SxbGenerateNo>();

            services.AddProductManagementAPIHttps(Configuration);

            #region autofac注入

            //var builder = new ContainerBuilder();

            //var deps = DependencyContext.Default;
            //var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            //var list = new List<Assembly>();
            //foreach (var lib in libs)
            //{
            //    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
            //    if (lib.Name.Contains("PMS."))
            //    {
            //        //var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
            //        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();//.AsSelf()
            //          //.OnActivated(e => Console.WriteLine(lib.Name + " OnActivated创建之后调用!"))
            //          //.OnRelease(e => Console.WriteLine(lib.Name + " OnRelease在释放占用的资源之前调用!"));
            //    }

            //    //var types = assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(DbContext)));
            //    //if(types != null)
            //    //{
            //    //    foreach (var item in types)
            //    //    {
            //    //        builder.RegisterType(item).AsSelf().InstancePerLifetimeScope();
            //    //    }
            //    //}
            //}
            //builder.Populate(services);
            //var applicationContainer = builder.Build();

            #endregion autofac注入


            #region CORS
            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
            #endregion

            //return new AutofacServiceProvider(applicationContainer);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            var list = new List<Assembly>();


            builder.RegisterGeneric(typeof(BaseRepository<>)).AsImplementedInterfaces().InstancePerDependency();
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //loggerFactory.AddPGAccessorLog(LogLevel.Trace);
                app.UseJudgeMiddleware();
                app.UseDeveloperExceptionPage();
                app.UseStatusCodeMiddleware();

                //app.UseStatusCodePagesWithReExecute("/errors/{0}");
                //app.UseStatusCodeMiddleware();
                //app.UseExceptionMiddleware();
                NLogBuilder.ConfigureNLog("nlog.Development.config");
            }
            else if (env.IsStaging())
            {
                app.UseStatusCodePagesWithReExecute("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.Staging.config");
                //订阅RabbitMQ
                //app.SubscribeRabbitMQ();
            }
            else if (env.IsProduction())
            {
                app.UseJudgeMiddleware();
                app.UseStatusCodePagesWithReExecute("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.config");
                //订阅RabbitMQ
                //不要在现网启动
                //app.SubscribeRabbitMQ();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDiffluenceMiddleware();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseCors("LimitRequests");
            app.UseAuthentication();
            app.UseRouting();//中间件中获取路由信息
            //app.UseEndpointRouting();//中间件中获取路由信息


            app.UseCompressHtmlMiddleware();

            app.UseLoggerMiddleware();
            //app.UseMiddleware<RedisProfilerMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}