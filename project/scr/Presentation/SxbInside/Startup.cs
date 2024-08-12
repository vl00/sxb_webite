using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Polly;
using Autofac;
using AutoMapper;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using PMS.School.Repository;
using Sxb.Inside.Models;
using ProductManagement.Framework.MSSQLAccessor;
using CommentApp.App_Start;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sxb.Inside.Middleware;
using Microsoft.AspNetCore.Authorization;
using Sxb.Inside.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.Serialize.Json;
using PMS.RabbitMQ.Handle;
using ProductManagement.Tool.Amap.Common;
using ProductManagement.Tool.Amap;
using ProductManagement.Tool.Email;
using System.Net.Http;
using PMS.UserManage.Repository;
using Newtonsoft.Json;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Cache.RedisProfiler;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.Redis.Serializer;
using ProductManagement.Framework.MongoDb;
using Sxb.Inside.Utils;
using PMS.Infrastructure.Repository;
using ProductManagement.API.Http;
using ProductManagement.API.Http.Common;
using ProductManagement.Framework.SearchAccessor;
using System.IO;
using Refit;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using iSchool.Internal.API.OperationModule;
using iSchool.Internal.API.UserModule;
using System.Web;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PMS.CommentsManage.Repository;
using Microsoft.Extensions.Hosting;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Aliyun.Common;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using iSchool.Internal.API.RankModule;
using PMS.Search.Elasticsearch;
using PMS.School.Infrastructure;
using Microsoft.AspNetCore.Http.Features;

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
                cfg.AddProfile(new VoMapperConfiguration(Configuration.GetSection("ImageSetting")["QueryImager"]));
            });
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());

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

            services.AddScopedMSSQLDbContext<TopicCircleDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("TopicCircleServer");
                c.Name = "TopicCircleServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
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
                c.Name = "SchoolSqlServer";
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
            services.AddScopedMSSQLDbContext<PaidQADBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("PaidQAServer");
                c.Name = "PaidQAServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<FinanceDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("FinanceServer");
                c.Name = "FinanceServer";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });
            services.AddScopedMSSQLDbContext<LogDBContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("LogServer");
                c.Name = "LogServer";
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

            //ES
            services.Configure<EsConfig>(Configuration.GetSection("SearchConfig"));
            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = Configuration.GetSection("SearchConfig").Get<ProductManagement.Framework.SearchAccessor.SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });

            //Mongo
            services.AddMongoDbAccessor(config =>
            {
                var dbs = Configuration.GetSection("MongoConfig").Get<List<MongoDbConfig>>();
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });

            //Statistics Mongo
            services.AddMongoDbAccessor<IStatisticsMongoProvider>(config =>
            {
                var dbs = Configuration.GetSection("StatisticsMongoConfig").Get<List<MongoDbConfig>>();
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });

            //Email
            services.AddScopedMailNotification(config =>
            {
                var mailConfig = Configuration.GetSection("MailNotification").Get<MailNotificationConfiguration>();
                config.SMTPService = mailConfig.SMTPService;
                config.SMTPPort = mailConfig.SMTPPort;
                config.EmailAccount = mailConfig.EmailAccount;
                config.EmailDisplayName = mailConfig.EmailDisplayName;
                config.Password = mailConfig.Password;
                config.EmailWhiteList = mailConfig.EmailWhiteList;
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
                        var redirectUrl = new UriBuilder
                        {
                            Host = "user.sxkid.com",
                            Port = 80,
                            Path = new PathString("/login"),
                            Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
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

            //用户中心接口
            services.Configure<UserSystemConfig>(Configuration.GetSection("UserSystemConfig"));
            services.AddHttpClient<IUserServiceClient, UserServiceClient>()
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
            //运营后台接口
            services.Configure<OperationConfig>(Configuration.GetSection("OperationConfig"));
            services.AddHttpClient<IOperationClient, OperationClient>()
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

            //Add OperationPlateform API Type HtppClient
            services.AddHttpClient<OperationApiServices>((sp, c) =>
            {
                var config = sp.GetService<IConfiguration>();
                string externalUrl = config.GetSection("ExternalInterface").GetValue<string>("OperationPlateFormBaseAddress");
                c.BaseAddress = new Uri(externalUrl);
            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            services.Configure<AliTextConfig>(Configuration.GetSection("AliTextConfig"));
            services.AddHttpClient<IText, Text>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Add User API Type HtppClient
            services.AddHttpClient<UserApiServices>((sp, c) =>
            {
                var config = sp.GetService<IConfiguration>();
                string externalUrl = config.GetSection("ExternalInterface").GetValue<string>("UserBaseAddress");
                c.BaseAddress = new Uri(externalUrl);

            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    UseDefaultCredentials = true,
                    UseCookies = true
                };

            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Add Rank API Type HttpClient
            services.AddHttpClient<RankApiServices>((sp, c) =>
            {
                var config = sp.GetService<IConfiguration>();
                string externalUrl = config.GetSection("ExternalInterface").GetValue<string>("RankAddress");
                c.BaseAddress = new Uri(externalUrl);
            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //依赖注入httpClient
            services.AddHttpClient();
            services.AddRouting(options => {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            }) ;

            services.AddRazorPages();
            services.AddControllersWithViews().AddControllersAsServices().AddNewtonsoftJson();


            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();


            services.AddEasyWeChat();
            services.AddAutoMapper(typeof(Startup));

            // If using Kestrel:
            //services.Configure<KestrelServerOptions>(options =>
            //{
            //    options.AllowSynchronousIO = true;
            //});

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddProductManagementAPIHttps(Configuration);

            //解决文件上传Request body too large
            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 536870912;//最大512M
            });

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
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region autofac注入

            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project");
            var list = new List<Assembly>();
            foreach (var lib in libs)
            {
                if (lib.Name.Contains("ProductManagement.Framework.Cache.Redis"))
                {
                    continue;
                }

                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                if (lib.Name.Contains("ProductManagement.Framework."))
                {
                    builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().SingleInstance();
                }
                //else if (lib.Name.Contains("PMS.CommentsManage.Repository"))
                //{
                //    builder.RegisterAssemblyTypes(assembly).InstancePerRequest();
                //}
                else
                {
                    //var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));

                    if (lib.Name.StartsWith("PMS."))
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


            //var applicationContainer = builder.Build();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                //loggerFactory.AddPGAccessorLog(LogLevel.Trace);

                app.UseDeveloperExceptionPage();
                app.UseStatusCodeMiddleware();

                //app.UseStatusCodePagesWithRedirects("/errors/{0}");
                //app.UseStatusCodeMiddleware();
                //app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.Development.config");
            }
            else if (env.IsStaging())
            {
                app.UseStatusCodePagesWithRedirects("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.Staging.config");
                //订阅RabbitMQ
                app.SubscribeRabbitMQ();
            }
            else if (env.IsProduction())
            {
                app.UseStatusCodePagesWithRedirects("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();
                //订阅RabbitMQ
                app.SubscribeRabbitMQ();
                NLogBuilder.ConfigureNLog("nlog.config");
            }

            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseAuthentication();


            //app.UseMiddleware<RedisProfilerMiddleware>();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseCors("LimitRequests");
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
