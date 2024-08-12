using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.UserInfoViewModel
{
    /// <summary>
    /// 我的关键词
    /// </summary>
    public class KeywordViewModel
    {
        /// <summary>
        /// 关注的学段类型
        /// </summary>
        public List<string> FollowGrades { get; set; }
        /// <summary>
        /// 关注的学校类型
        /// </summary>
        public List<string> FollowSchoolTypes { get; set; }
        /// <summary>
        /// 关注的住宿情况
        /// </summary>
        public List<string> FollowLoadings { get; set; }
    }
}
