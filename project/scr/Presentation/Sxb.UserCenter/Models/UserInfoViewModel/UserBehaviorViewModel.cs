using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.UserInfoViewModel
{
    public class UserBehaviorViewModel
    {
        /// <summary>
        /// 关注数
        /// </summary>
        public int FollowTotal { get; set; }
        /// <summary>
        /// 动态数
        /// </summary>
        public int DynamicTotal { get; set; }
        /// <summary>
        /// 最近浏览
        /// </summary>
        public int RecentBrowseTotal { get; set; }
    }
}
