using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Tool.Models.QRCoder
{
    public class GetMinAppQRCodeRequest
    {
        [Description("应用标识枚举值")]
        public string AppName { get; set; } = "app";
        [Description("页面路径")]
        public string Page { get; set; }
        [Description("场景值")]
        public string Scene { get; set; }

        [Description("二维码宽度")]
        public int Width { get; set; } = 200;
    }
}
