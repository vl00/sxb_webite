using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class FollowFansTotal
    {
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int Fans { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int Follow { get; set; }
    }

    /// <summary>
    /// 我的页面 
    /// </summary>
    public class MydynamicTotal 
    {
        /// <summary>
        /// 关注数
        /// </summary>
        public int Follow { get; set; }
        /// <summary>
        /// 动态数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 最近浏览数
        /// </summary>
        public int History { get; set; }
    }
}
