using Autofac;
using AutoMapper;
using CommentApp.App_Start;
using iSchool.API;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog.Web;
using PMS.CommentsManage.Repository;
using PMS.Infrastructure.Repository;
using PMS.RabbitMQ.Handle;
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
using ProductManagement.Tool.Email;
using Sxb.GenerateNo;
using Sxb.Web.Filters;
using Sxb.Web.Middleware;
using Sxb.Web.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using PMS.Search.Elasticsearch;
using Microsoft.AspNetCore.StaticFiles;
using Sxb.GenerateNo;
using ProductManagement.Framework.MongoDb;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using PMS.OperationPlateform.Repository;
using Sxb.Web.Areas.Common.Controllers;
using Sxb.Web.Areas;

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
            this.env = env;
        }

        private IHostEnvironment env;
        public IConfigurationRoot Configuration { get; }
        private MapperConfiguration _mapperConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加响应压缩
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });

            services.AddEventBus(Configuration);
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.Configure<AliTextConfig>(Configuration.GetSection("AliTextConfig"));
            services.AddHttpClient<IText, Text>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            services.Configure<ImageSetting>(Configuration.GetSection("ImageSetting"));

            services.Configure<MongoSetting>(Configuration.GetSection("MongoDBConfig"));

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new VoMapperConfiguration(Configuration.GetSection("ImageSetting")["QueryImager"]));
                cfg.AddProfile(new PMS.TopicCircle.Application.Dtos.DtoMapperConfiguration());
                cfg.AddProfile(new Sxb.Web.Areas.PaidQA.Models.ResultMapperConfiguration());
                cfg.AddProfile(new Sxb.Web.Areas.Coupon.Models.ResultMapperConfiguration());
                cfg.AddProfile(new Sxb.Web.Areas.Article.Models.ResultMapperConfiguration());
                cfg.AddProfile(new PMS.OperationPlateform.Domain.ResultMapperConfiguration());
                cfg.AddProfile(new PMS.PaidQA.Domain.Dtos.ResultMapperConfiguration());
                cfg.AddProfile(new PMS.SignalR.Clients.PaidQAClient.Models.ResultMapperConfiguration());
                


            });
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());

            //注入配置Options
            services.AddConfig(this.Configuration);
            

            //EF
            services.AddDbContext<CommentsManageDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProductServer")));

            //services.AddDbContext<CommentsManageDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("ProductServer")));

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
            services.AddScopedMSSQLDbContext<LiveDBContext>(c =>
            {                
                var SQLConfig = Configuration.GetConnectionString("ISchoolLive");
                c.Name = "LiveServer";
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

            //注入MediateR对象
            //var mediatRBrowsePageEventAssembly = Assembly.GetAssembly(typeof(BrowsePageEvent));
            //var mediatRHistoryHandlerAssembly = Assembly.GetAssembly(typeof(HistoryHandler));
            //var mediatRRequestAssembly = Assembly.Load("PMS.MediatR.Handle");
            services.AddMediatR(typeof(Startup));

            //RabbitMQ
            services.AddProductManagementRabbitMQ(option =>
            {
                var config = Configuration.GetSection("rabbitMQSetting").Get<RabbitMQOption>();
                option.AmqpUris = config.AmqpUris;
                option.Uri = config.Uri;
                option.ExtName = config.ExtName;
            }, new ProductManagement.Framework.Serialize.Json.NewtonsoftSerializer())
            .ScanMessage(typeof(SyncSchoolScoreHandler).Assembly, this.GetType().Assembly);

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
            services.AddMongoDbAccessor<IStatisticsMongoProvider> (config =>
            {
                var dbs = Configuration.GetSection("StatisticsMongoConfig").Get<List<MongoDbConfig>>();
                config.ConfigName = dbs[0].ConfigName;
                config.Database = dbs[0].Database;
                config.ConnectionString = dbs[0].ConnectionString;
                config.WriteCountersign = dbs[0].WriteCountersign;
            });
            
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
            //ES
            services.Configure<EsConfig>(Configuration.GetSection("SearchConfig"));
            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = Configuration.GetSection("SearchConfig").Get<ProductManagement.Framework.SearchAccessor.SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });

            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";
            //本地模拟登录
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            //{
            //    o.Cookie.HttpOnly = true;
            //    o.Cookie.Name = "iSchoolAuth";
            //    o.Cookie.Domain = ".sxkid.com";
            //    o.Cookie.Path = "/";
            //    //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"c:\shared-auth-ticket-keys\"));
            //    o.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
            //    o.LoginPath = new PathString("/TestLogin/Index");
            //});

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
                        //var redirectUrl = new UriBuilder
                        //{
                        //    Host = "user.sxkid.com",
                        //    Port = 80,
                        //    Path = new PathString("/login"),
                        //    Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
                        //};
                        var userServiceUrl = (Configuration.GetSection("UserSystemConfig").GetValue<string>("ServerUrl") ?? "").Replace("https://", "").Replace("http://", "");
                        var redirectUrl = new UriBuilder
                        {
                            Scheme = "https",
                            Host = userServiceUrl,
                            Path = new PathString("/login/"),
                            Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
                        };

                        //var redirectUrl = $"{Configuration.GetSection("UserSystemConfig")?.GetSection("ServerUrl")?.Value}/login/login.html{QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value}";

                        context.Response.Redirect(redirectUrl.ToString());
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

            //获取客户端IP地址
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //HTTP 请求
            services.AddHttpClient<WxworkInviteAvitityController>(c =>
            {
                string externalUrl = this.Configuration.GetSection("WXWorkClientConfig").GetValue<string>("ServerUrl");
                c.BaseAddress = new Uri(externalUrl);
            });

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

            //直播接口
            services.Configure<LiveConfig>(Configuration.GetSection("LiveConfig"));
            services.AddHttpClient<ILiveServiceClient, LiveServiceClient>()
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
            //上学帮文件服务中心接口
            services.Configure<FileConfig>(Configuration.GetSection("FileConfig"));
            services.AddHttpClient<IFileServiceClient, FileServiceClient>()
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

            //用户推荐接口
            services.Configure<UserRecommendConfig>(Configuration.GetSection("UserRecommend"));
            services.AddHttpClient<IUserRecommendClient, UserRecommendClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.AddProductManagementAPIHttps(this.Configuration);
            services.AddHttpClient<IHtmlServiceClient, HtmlServiceClient>()
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
            services.AddHttpClient<RankApiServices>(c =>
            {
                string externalUrl = this.Configuration.GetSection("ExternalInterface").GetValue<string>("RankAddress");
                c.BaseAddress = new Uri(externalUrl);
            })
             .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));


            //依赖注入httpClient
            services.AddHttpClient();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            //构建上学帮原有项目的API CLient
            string baseAddress = this.Configuration.GetSection("ExternalInterface").GetValue<string>("ISchool");
            services.AddSxbFileService("iSchoolCenterClient", baseAddress);

            services.AddRazorPages();
            services.AddControllersWithViews().AddControllersAsServices()
                .AddNewtonsoftJson()//将Web中Json序列化库替换为Newtonsoft
                .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    ResponseResult response = new ResponseResult()
                    {
                        Succeed = false,
                        status = ResponseCode.Failed,
                        Msg = "无效参数"
                    };
                    return new JsonResult(response);
                };
            }); ;


            //services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            });
            // 关闭netcore自动处理参数校验机制
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            services.AddAutoMapper(typeof(Startup));



            if (env.IsDevelopment())
            {
                services.AddRazorPages().AddRazorRuntimeCompilation();
            }
            #region autofac注入

            services.AddEasyWeChat();

            //var builder = new ContainerBuilder();
            //builder.Populate(services);


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
            //                                                                          //.OnActivated(e => Console.WriteLine(lib.Name + " OnActivated创建之后调用!"))
            //                                                                          //.OnRelease(e => Console.WriteLine(lib.Name + " OnRelease在释放占用的资源之前调用!"));
            //    }
            //}


            //var applicationContainer = builder.Build();
            //var serviceProvider = new AutofacServiceProvider(applicationContainer);

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
            //return serviceProvider;


            services.AddSwaggerDocument();

   

            services.AddCustomSignalR();

            services.AddSingleton<ISxbGenerateNo, SxbGenerateNo>();

            services.AddTencentSms();

            services.AddWeChatQRCallBackHandles();

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
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                //loggerFactory.AddPGAccessorLog(LogLevel.Trace);

                app.UseDeveloperExceptionPage();
                app.UseStatusCodeMiddleware();

                //app.UseStatusCodePagesWithRedirects("/errors/{0}");
                //app.UseStatusCodeMiddleware();
                //app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.Development.config");

                //订阅RabbitMQ
                //app.SubscribeRabbitMQ();

                //增加swagger便于调试
                app.UseOpenApi(configure=> {
                    configure.PostProcess = (doc, request) =>
                    {
                        doc.Schemes = new[] { NSwag.OpenApiSchema.Https, NSwag.OpenApiSchema.Http,NSwag.OpenApiSchema.Ws,NSwag.OpenApiSchema.Wss};
                    };
                });
                app.UseSwaggerUi3(configure=> { });
            }
            else if (env.IsStaging())
            {
                app.UseStatusCodePagesWithRedirects("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                NLogBuilder.ConfigureNLog("nlog.Staging.config");
                //订阅RabbitMQ
                //app.SubscribeRabbitMQ();

                //增加swagger便于调试
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else if (env.IsProduction())
            {
                //app.UseJudgeMiddleware();
                app.UseStatusCodePagesWithRedirects("/errors/{0}");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();
                //订阅RabbitMQ
                //不要在现网启动
                //app.SubscribeRabbitMQ();
                NLogBuilder.ConfigureNLog("nlog.config");
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //app.UseDiffluenceMiddleware();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseCommentQuestionUrlMiddleware();  //写点评 | 提问页url重写

            //app.UseEndpointRouting();//中间件中获取路由信息


            //app.UseMiddleware<RedisProfilerMiddleware>();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseCors("LimitRequests");
            app.UseRouting();
            app.UseLoggerMiddleware();
            app.UseAuthorization();
            app.UseUserBlockCheckMiddleware();
            app.UseWeixinAutoLoginMiddleware();


            string pkPath = Path.Combine(env.WebRootPath, "dist/school-pk");
            if (Directory.Exists(pkPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    //把静态目录映射为某一个特定的 URL 地址目录下面
                    RequestPath = "/pk",
                    FileProvider = new PhysicalFileProvider(pkPath)
                });
            }
            string livePath = Path.Combine(env.WebRootPath, "dist/live");
            if (Directory.Exists(livePath))
            {
                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".vue"] = "application/x-javascript";

                app.UseStaticFiles(new StaticFileOptions
                {
                    //把静态目录映射为某一个特定的 URL 地址目录下面
                    RequestPath = "/live",
                    FileProvider = new PhysicalFileProvider(livePath),
                    ContentTypeProvider = provider
                });
            }

            string topicPath = Path.Combine(env.WebRootPath, "dist/topic");
            if (Directory.Exists(topicPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    //把静态目录映射为某一个特定的 URL 地址目录下面
                    RequestPath = "/topic",
                    FileProvider = new PhysicalFileProvider(topicPath)
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<PMS.SignalR.Hubs.PaidQAHub.OrderChatHub>("/paidqa/orderchathub");
            });
        }
    }
}