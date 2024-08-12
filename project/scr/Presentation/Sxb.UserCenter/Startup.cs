using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Repository;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Middleware;
using ProductManagement.Framework.MSSQLAccessor;
using PMS.Infrastructure.Repository;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.RedisProfiler;
using PMS.School.Infrastructure;
using Newtonsoft.Json;
using Sxb.UserCenter.Utils;
using ProductManagement.Framework.Cache.Redis;
using Autofac;
using System.Reflection;
using System.Runtime.Loader;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.Services;
using AutoMapper;
using ProductManagement.Tool.Amap.Common;
using ProductManagement.Tool.Amap;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using PMS.CommentsManage.Repository;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Framework.SearchAccessor;
using Microsoft.Extensions.FileProviders;
using ProductManagement.API.Aliyun.Common;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using ProductManagement.API.Http.Common;
using Sxb.UserCenter.Common;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sxb.UserCenter.Models.CommonViewModel;
using System.Net.Http;
using iSchool.Internal.API.RankModule;
using iSchool.Internal.API.UserModule;
using PMS.RabbitMQ.Handle;
using NLog.Web;
using ProductManagement.Framework.RabbitMQ;
using System.Runtime.InteropServices;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using PMS.TopicCircle.Application.Dtos;
using PMS.Search.Elasticsearch;
using ProductManagement.UserCenter.BaiduCommon.Common;
using ProductManagement.UserCenter.BaiduCommon;
using ProductManagement.Framework.MongoDb;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProductManagement.API.Http;

namespace Sxb.UserCenter
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                p.StartInfo.FileName = "net";
                p.StartInfo.Arguments = @"use \\10.1.0.16 /user:sxkid.com\IISAuth SxbAuth$8888";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();

                filePath = Configuration.GetValue<string>("AuthTicketPath");
            }

            services.Configure<AliTextConfig>(Configuration.GetSection("AliTextConfig"));
            services.AddHttpClient<IText, Text>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            

            services.AddScopedMSSQLDbContext<UserDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("iSchoolUser");
                c.Name = "iSchoolUser";
                c.LoadBalancer = SQLConfig;
                c.RealDbConnectionStrings = new List<string> { SQLConfig };
            });

            services.AddScopedMSSQLDbContext<JcDbContext>(c =>
            {
                var SQLConfig = Configuration.GetConnectionString("iSchool");
                c.Name = "iSchool";
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

            services.AddDbContext<CommentsManageDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProductServer")));

            services.Configure<ImageSetting>(Configuration.GetSection("ImageSetting"));

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

            services.AddHttpClient<iSchool.Data.API.ISchoolDataClient>(c =>
            {
                string iSchoolDataBaseAddress = this.Configuration.GetSection("ExternalInterface").GetValue<string>("iSchoolDataBaseAddress");
                c.BaseAddress = new Uri(iSchoolDataBaseAddress);
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

            //ES
            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = Configuration.GetSection("SearchConfig").Get<ProductManagement.Framework.SearchAccessor.SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<AppInfos>(Configuration.GetSection("AppInfos"));
            services.Configure<IsHost>(Configuration.GetSection("IsHost"));
            services.Configure<ConnectionOptions>(Configuration.GetSection("ConnectionStrings"));

            //new iSchoolRedisHelper(Configuration.GetSection("Redis").Get<RedisOptions>());
            new DataAccess(Configuration.GetSection("ConnectionStrings").Get<ConnectionOptions>());
            new MD5Helper(Configuration.GetValue<string>("MD5Tail"));
            //BLL.APIBLL.IsHost = Configuration.GetSection("IsHost").Get<Models.IsHost>();

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

                //options.LoginPath = new PathString("/home/login");
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = Configuration.GetValue<string>("AuthCookieName");
                options.Cookie.Domain = ".sxkid.com";
                options.Cookie.Path = "/";
                options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
            }
            )
            .AddJwtBearer(o => {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    //是否验证发行人
                    ValidateIssuer = true,
                    ValidIssuer = "sxkid.com",//发行人
                                          //是否验证受众人
                    ValidateAudience = true,
                    ValidAudience = "api.auth",//受众人
                                             //是否验证密钥
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("q2xiARx$4x3TKqBJ")),

                    ValidateLifetime = true, //验证生命周期
                    RequireExpirationTime = true, //过期时间
                };
            }); ;
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddProductManagementAPIHttps(this.Configuration);
            services.AddHttpClient<IAccountService, AccountService>();
            services.AddHttpClient();

            services.Configure<BaiduConfig>(Configuration.GetSection("BaiduConfig"));
            services.AddHttpClient<IBaiduOAuthClient, BaiduOAuthClient>()
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.Configure<AmapConfig>(Configuration.GetSection("AmapConfig"));
            services.AddHttpClient<IAmapClient, AmapClient>()
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

            services.Configure<OperationConfig>(Configuration.GetSection("OperationConfig"));
            services.AddHttpClient<IOperationClient, OperationClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));


            services.Configure<FileConfig>(Configuration.GetSection("FileConfig"));
            services.AddHttpClient<IFileServiceClient, FileServiceClient>()
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

            //services.AddMvc(options => {
            //    options.Filters.Add(new Controllers.AuthorizeAttribute());
            //    options.EnableEndpointRouting = false;
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews(options => {
                //options.Filters.Add(new Controllers.AuthorizeAttribute());
                options.EnableEndpointRouting = false;
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddHttpClient();

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

            //services.AddAutoMapper(typeof(AutoMapperProfileConfiguration));
            services.AddSingleton(sp => 
                new MapperConfiguration(config => {
                    config.AddProfile(new AutoMapperProfileConfiguration());
                    config.AddProfile(new DtoMapperConfiguration());
                    config.AddProfile(new PMS.OperationPlateform.Domain.ResultMapperConfiguration());
                }).CreateMapper());

            services.AddControllers().AddNewtonsoftJson();

            services.Configure<WeChatAppConfig>(Configuration.GetSection("WeChatAppConfig"));
            services.AddHttpClient<IWeChatAppClient, WeChatAppClient>()
                 .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));
            services.AddEasyWeChat();
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
                                                                                      //.OnActivated(e => Console.WriteLine(lib.Name + " OnActivated����֮�����!"))
                                                                                      //.OnRelease(e => Console.WriteLine(lib.Name + " OnRelease���ͷ�ռ�õ���Դ֮ǰ����!"));
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStatusCodeMiddleware();
                app.UseDeveloperExceptionPage();


                //app.UseExceptionMiddleware();
            }
            else if (env.IsStaging())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                //订阅RabbitMQ
                app.SubscribeRabbitMQ();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodeMiddleware();
                app.UseExceptionMiddleware();

                //不要在现网启动
                //app.SubscribeRabbitMQ();
            }
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            
            app.UseRouting();
            app.UseCors("LimitRequests");
            app.UseAuthorization();

            app.UseHttpsRedirection();
            //DefaultFilesOptions defaultFiles = new DefaultFilesOptions();
            //defaultFiles.DefaultFileNames.Add("/dist/mine/mine.html");
            //app.UseDefaultFiles(defaultFiles);


            //var provider = new FileExtensionContentTypeProvider();
            //provider.Mappings[".vue"] = "application/x-javascript";

            //app.MapWhen(predicate => 
            //{
            //   return predicate.Request.Path.StartsWithSegments("/mine") || predicate.Request.Path.StartsWithSegments("/login"); 
            //},
            //configuration => 
            //{
            //    configuration
            //    .UseStaticFiles(new StaticFileOptions
            //    {
            //        RequestPath = "/mine",
            //        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/dist/mine")),
            //        ContentTypeProvider = provider
            //    })
            //    .UseStaticFiles(new StaticFileOptions {
            //        RequestPath = "/login",
            //        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/dist/login")),
            //        ContentTypeProvider = provider
            //    });
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
