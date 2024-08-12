using Sxb.Web.Models.Tag;
using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Comment
{
    /// <summary>
    /// 前端点评展示
    /// </summary>
    public class CommentRootVo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid SchoolId { get; set; }

        public Guid SchoolSectionId { get; set; }

        public string UserName { get; set; }
        public string UserHeadImage { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Images { get; set; }

        public string Content { get; set; }

        public string State { get; set; }

        public bool IsTop { get; set; }

        public CommentScore Score { get; set; }

        public DateTime createTime { get; set; }
        public bool isLike { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }

        public string SchoolBranch { get; set; }
        public int StartTotal { get; set; }
        public class CommentScore
        {
            /// <summary>
            /// 是否就读
            /// </summary>
            public bool IsAttend { get; set; }

            /// <summary>
            /// 总分
            /// </summary>
            public decimal AggScore { get; set; }


            /// <summary>
            /// 师资力量分
            /// </summary>
            public decimal TeachScore { get; set; }

            /// <summary>
            /// 硬件设施分
            /// </summary>
            public decimal HardScore { get; set; }

            /// <summary>
            /// 环境周边分
            /// </summary>
            public decimal EnvirScore { get; set; }

            /// <summary>
            /// 学风管理分
            /// </summary>
            public decimal ManageScore { get; set; }

            /// <summary>
            /// 校园生活分
            /// </summary>
            public decimal LifeScore { get; set; }
        }

    }
}
