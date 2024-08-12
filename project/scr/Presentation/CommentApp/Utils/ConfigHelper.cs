using CommentApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Utils
{
    public class ConfigHelper
    {
        /// <summary>
        /// _fwh
        /// </summary>
        /// <returns></returns>
        public static string GetFwhSuffix()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            string fwh_suffix = configuration.GetSection("Wx").GetValue<string>("fwh_suffix");
            return fwh_suffix;
        }

        /// <summary>
        /// fwh
        /// </summary>
        /// <returns></returns>
        public static string GetFwh()
        {
            return GetFwhSuffix().TrimStart('_');
        }


        public static string GetWxMiniProgramAppId()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            string appid = configuration.GetSection("Wx_MiniProgram").GetValue<string>("AppID");
            return appid;
        }
        public static string GetWxMpOrgAppId()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            string appid = configuration.GetSection("Wx_MiniProgram_Org").GetValue<string>("AppID");
            return appid ?? "wx0da8ff0241f39b11";
        }
        
        public static string GetHost()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            string Host = configuration["Host"];
            return Host;

        }
        public static int GetHostEnviroment()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            var env = configuration["SxbEnv"];
            if (null != env && !string.IsNullOrEmpty(env))
                return Convert.ToInt32(configuration["SxbEnv"]);
            else
                return 0;
           

        }
        public static string GetUserHost()
        {
            IConfiguration configuration = Program.GlobalHost.Services.GetService<IConfiguration>();
            string Host = configuration.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");
            return Host;
        }

        
    }
}
