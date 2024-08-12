using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WeChat.Model;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    public class SubscribeCallBackFormRequest
    {
        [Description("微信OpenId")]
        public string OpenId { get; set; }

        [Description("应用ID")]
        public string AppId { get; set; }

        [Description("应用名称，用于获取AccessToken之类，上学帮内部唯一标识")]
        public string AppName { get; set; }
        public MiniProgramCardInfo MiniProgramCardInfo { get; set; }

        [Description("微信回调事件类型")]
        public WeChatEventEnum? Event { get; set; }

        public override string ToString()
        {
            List<string> result = new List<string>();
            result.Add($"OpenId:{OpenId};AppId:{AppId};");
            if (MiniProgramCardInfo != null)
            {
                result.Add("MiniProgramCardInfo:" + Newtonsoft.Json.JsonConvert.SerializeObject(MiniProgramCardInfo));
            }
            else
            {
                result.Add("MiniProgramCardInfo:null");

            }
            return string.Join("\n", result);
        }

    }
}
