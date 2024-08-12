using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.App_Start;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using ProductManagement.Web.ModelVo;
using Autofac.Extensions.DependencyInjection;
using NLog.Web;
using Microsoft.Extensions.Hosting;

namespace ProductManagement.Web
{
    public class Program
    {
        public static Func<string, int> func;

        public static void Main(string[] args)
        {
            //配置初始化
            InitConfig.Init();
            try
            {
                 

                //PartTimeJobAdminDto obj = new PartTimeJobAdminDto();
                //obj.Role = 1;
                //obj.Name = "兼职";

                //PartTimeJobAdminDto obj1 = new PartTimeJobAdminDto();
                //obj1.Role = 2;
                //obj1.Name = "兼职领队";

                //PartTimeJobAdminDto obj2 = new PartTimeJobAdminDto();
                //obj2.Role = 3;
                //obj2.Name = "供应商";

                //PartTimeJobAdminDto obj4 = new PartTimeJobAdminDto();
                //obj4.Role = 4;
                //obj4.Name = "管理员";


                //List<PartTimeJobAdminDto> list = new List<PartTimeJobAdminDto>();
                //list.Add(obj);
                //list.Add(obj1);
                //list.Add(obj2);
                //list.Add(obj4);
                //List<PartTimeJobAdminVo> vo = Mapper.Map<List<PartTimeJobAdminDto>, List<PartTimeJobAdminVo>>(list);
                //int a = 1;
                //ITestService userInfoServices = IocServiceContainer.Current.Resolve<ITestService>();
                //TestVo testVo = new TestVo();
                //testVo.Id = Guid.NewGuid();
                //testVo.Account = "sa";
                //testVo.Password = "1234";
                //testVo.AddTime = DateTime.Now;

                //userInfoServices.GetList();

                //TestObject testObject = Mapper.Map<TestVo, TestObject>(testVo);
                //object ob = userInfoServices.GetList(x => x.Account == "123" & x.Password == "123");
                //userInfoServices.Insert(testObject);


                //userInfoServices.Delete(Guid.Parse("0ECFF90F-E986-432D-9C76-F938B1C3C736"));
                ////初始化log4net日志

                //    Logger.Log("写入");
            }
            catch (Exception)
            {
                throw;
            }
            //CreateWebHostBuilder(args).Build().Run();

            var host = CreateWebHostBuilder(args).Build();

            host.Run();


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
