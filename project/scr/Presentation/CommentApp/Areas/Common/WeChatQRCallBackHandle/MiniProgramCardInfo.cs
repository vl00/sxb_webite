using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    /// <summary>
    /// 小程序卡片信息
    /// </summary>
    public class MiniProgramCardInfo
    {
        public string Title { get; set; }

        public string AppId { get; set; }

        public string ThumbMediaId { get; set; }
    }
}
