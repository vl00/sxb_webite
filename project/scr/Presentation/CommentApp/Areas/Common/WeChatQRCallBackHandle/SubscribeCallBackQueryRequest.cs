using Nest;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Ad;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic;
using Sxb.Web.Filters;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    public class SubscribeCallBackQueryRequest
    {

        [Description("业务ID")]
        public Guid? DataId { get; set; }

        [Description("业务链接")]
        public string DataUrl { get; set; }

        [Description("额外数据")]
        public string DataJson { get; set; }

        [Description("回调类型")]
        public SubscribeQRCodeType type { get; set; }

        [Description("使用微信小程序客服消息进行回调转发至公众号时，需要填写该字段。")]
        public SubscribeQRCodeType ForwardType { get; set; }

        [Description("声明该请求是转发自微信小程序客服消息。")]
        public bool IsForwardByWXMP { get; set; } = false;

        [Description("转发过来的APPID。")]
        public string ForwardData { get; set; }

        public string ToQueryString()
        {
            string dataUrl = string.Empty;
            if (!string.IsNullOrEmpty(this.DataUrl))
            {
                if (this.DataUrl.IndexOf("//") != -1)
                {
                    dataUrl = HttpUtility.UrlEncode(this.DataUrl);
                }
                else
                {
                    dataUrl = this.DataUrl;
                }
            }
            var dataJson = HttpUtility.UrlEncode(this.DataJson);
            return $"DataId={this.DataId}&DataUrl={dataUrl}&type={this.type}&forwardType={ForwardType}&IsForwardByWXMP={IsForwardByWXMP}&ForwardData={ForwardData}&DataJson={dataJson}";
        }


        public override string ToString()
        {
            return $"DataID:{this.DataId};DataUrl:{this.DataUrl};Type:{this.type};forwardType={ForwardType};IsForwardByWXMP={IsForwardByWXMP};ForwardData={ForwardData};DataJson={DataJson}";
        }

    }













}
