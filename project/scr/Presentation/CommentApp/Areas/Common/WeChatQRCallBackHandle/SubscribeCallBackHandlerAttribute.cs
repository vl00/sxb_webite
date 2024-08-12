using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    public class SubscribeCallBackHandlerAttribute : Attribute
    {
        public Type THandler { get; set; }
        public SubscribeCallBackHandlerAttribute(Type handler)
        {
            THandler = handler;
        }
    }
}
