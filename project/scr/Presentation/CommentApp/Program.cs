using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommentApp.App_Start;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.Web.Models.Comment;
using NLog.Web;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CommentApp
{
    public class Program
    {
        internal static IHost GlobalHost;
        public static void Main(string[] args)
        {
            GlobalHost =  CreateWebHostBuilder(args).Build();
           
            GlobalHost.Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//使用AutoFac做IOC和AOP
            .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder//.UseKestrel().UseUrls("https://*:5001")
                .ConfigureServices(services =>
                {
                    //iSchool.LngLatLocation.Init_With_Dapper();
                    Dapper.SqlMapper.AddTypeHandler(typeof(iSchool.SchFType0), new iSchool.SchFType0TypeHandler());
                    //try
                    //{
                    //    var jsonstr = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schtype.json"));
                    //    iSchool.SchFTypeUtil.Dict = PMS.School.Infrastructure.Common.JsonHelper.JSONToObject<Dictionary<string, string>>(jsonstr);
                    //    iSchool.SchFTypeUtil.Dict.Remove("_none_");
                    //}
                    //catch { }

                    PMS.School.Infrastructure.JsonNetExtensions.SerializerSettings.Converters.Add(new PMS.School.Infrastructure.SchFType0JsonConverter());
                })
                .UseStartup<Startup>()
                .UseNLog();
             });
                
    }
}
