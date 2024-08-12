using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models.User;
using Sxb.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Comment
{
    /// <summary>
    /// 点评
    /// </summary>
    public class CommentList
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        public string No { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        /// <summary>
        /// 点评标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 学校id 
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点评状态
        /// </summary>
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 点评打分
        /// </summary>
        public SchoolCommentScoreViewModel Score { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string CreateTime { get; set; }


        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 点亮
        /// </summary>
        public int StartTotal { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }

        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool IsRumorRefuting { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }


        public SchoolInfoVo School { get; set; }
        public UserInfoVo UserInfo { get; set; }


        public List<SchoolCommentReplyVo> CommentReplies { get; set; }
        public class SchoolInfoVo
        {
            public Guid SchoolSectionId { get; set; }
            public Guid SchoolId { get; set; }
            public string SchoolName { get; set; }
            //学校类型
            public SchoolType SchoolType { get; set; }
            /// <summary>
            /// 寄宿类型
            /// </summary>
            public int LodgingType { get; set; }
            /// <summary>
            /// 寄宿类型描述
            /// </summary>
            public string LodgingReason { get; set; }
            /// <summary>
            /// 是否上学帮认证
            /// </summary>
            public bool IsAuth { get; set; }
            /// <summary>
            /// 学校总平均分
            /// </summary>
            public decimal SchoolAvgScore { get; set; }
            /// <summary>
            /// 前端展示星星
            /// </summary>
            public int SchoolStars { get; set; }
            /// <summary>
            /// 学校总点评数
            /// </summary>
            public int CommentTotal { get; set; }
            /// <summary>
            /// 是否存在点评
            /// </summary>
            public bool IsExists { get; set; }
            public string SchoolBranch { get; set; }

            public int SchoolNo { get; set; }
            public string ShortSchoolNo
            {
                get
                {
                    return UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
                }
            }
        }
    }
}
