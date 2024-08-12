using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.OperationPlateform.Application.IServices;
using PMS.PaidQA.Application.Services;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using ProductManagement.API.Http.Interface;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Ad;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{


    /// <summary>
    /// 微信关注回调处理者对象构造工厂
    /// </summary>
    public static class IServiceCollectionExtension
    {

        /// <summary>
        /// 注入微信回调的各个处理器
        /// </summary>
        /// <param name="services"></param>
        public static void AddWeChatQRCallBackHandles(this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IServiceCollectionExtension));
            IEnumerable<Type> handles = assembly.GetTypes().Where(t => t.FullName.StartsWith("Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers", StringComparison.CurrentCultureIgnoreCase));
            if (handles?.Any() == true)
            {
                foreach (var handle in handles)
                {
                    services.AddTransient(handle);
                }
            }
        
        }


    }
}
