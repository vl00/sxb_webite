using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.Common
{
    public class AppInfos
    {
        public AppInfo Weixin { get; set; }
        public AppInfo Weixin_App { get; set; }
        public AppInfo Weixin_Web { get; set; }
        public AppInfo Weixin_MiniProgram_Sxkid { get; set; }
        public AppInfo Weixin_MiniProgram_Lecture { get; set; }
        public AppInfo Weixin_MiniProgram_Org { get; set; }
        public AppInfo Weixin_MiniProgram_Shop { get; set; }
        public AppInfo Weixin_MiniProgram_Pay { get; set; }
        public AppInfo QQ_App { get; set; }
        public AppInfo QQ_Web { get; set; }
    }

    public class AppInfo
    {
        public string AppName { get; set; }
        public string AppID { get; set; }
        public string AppSecret { get; set; }
    }
}
