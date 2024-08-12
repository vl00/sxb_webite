using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.CommentViewModel
{
    /// <summary>
    /// 点评信息
    /// </summary>
    public class CommentInfoViewModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid ExId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid Sid { get; set; }
        /// <summary>
        /// 是否为精华点评
        /// </summary>
        public bool IsEssence { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyTotal { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 点评图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 写入日期
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 检测是否关注该点评
        /// </summary>
        public bool IsCollection { get; set; }
        /// <summary>
        /// 学校卡片
        /// </summary>
        public SchoolCommentCardViewModel School { get; set; }
        /// <summary>
        /// 写入用户
        /// </summary>
        public UserViewModel UserInfo { get; set; }
        /// <summary>
        /// 用户评分
        /// </summary>
        public CommentScoreViewModel CommentScore { get; set; }
    }
}
