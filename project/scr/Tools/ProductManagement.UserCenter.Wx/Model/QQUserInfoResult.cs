using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.TencentCommon.Model
{
    public class QQUserInfoResult : QQRet
    {
        public bool is_lost { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string year { get; set; }
        public string constellation { get; set; }
        public string figureurl { get; set; }
        public string figureurl_1 { get; set; }
        public string figureurl_2 { get; set; }
        public string figureurl_qq_1 { get; set; }
        public string figureurl_qq_2 { get; set; }
        public string figureurl_qq { get; set; }
        public byte figureurl_type { get; set; }
        public string is_yellow_vip { get; set; }
        public string vip { get; set; }
        public int yellow_vip_level { get; set; }
        public int level { get; set; }
        public string is_yellow_year_vip { get; set; }
    }
}
