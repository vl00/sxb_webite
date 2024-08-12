using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    /// <summary>
    /// 定义默认的微信关注回调的处理者
    /// </summary>
    public interface ISubscribeCallBackHandler
    {
        Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest);
    }
}
