using System;
using Sxb.Web.Models.User;

namespace Sxb.Web.ViewModels.Comment
{
    public class SchoolCommentViewModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 评论者ID
        /// </summary>
        public Guid CommentUserId { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点评审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public int State { get; set; }

        public int PostUserRole { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 写入日期
        /// </summary>
        public string AddTime { get; set; }

        public UserInfoVo UserInfo { get; set; }
        public SchoolCommentScoreViewModel SchoolCommentScore { get; set; }
    }
}
