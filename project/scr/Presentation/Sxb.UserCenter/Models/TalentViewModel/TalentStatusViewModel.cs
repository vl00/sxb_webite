using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.TalentViewModel
{
    /// <summary>
    /// 达人及子动态
    /// </summary>
    public class TalentStatusViewModel
    {
        public string TalentId { get; set; }

        public string NickName { get; set; }
        public string StaffName { get; set; }

        /// <summary>
        /// 总点赞数量
        /// </summary>
        public long LikeTotal { get; set; }
        /// <summary>
        /// 总收藏数量
        /// </summary>
        public long FollowTotal { get; set; }
        /// <summary>
        /// 回答问题数量
        /// </summary>
        public long QATotal { get; set; }
        /// <summary>
        /// 发布文章数量
        /// </summary>
        public long ArticleTotal { get; set; }
        /// <summary>
        /// 课程直播数量
        /// </summary>
        public long LectureTotal { get; set; }
        /// <summary>
        /// 子达人状态
        /// </summary>
        public List<TalentStatusViewModel> Children { get; set; }

    }
}
