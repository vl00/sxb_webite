using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace PMS.OperationPlateform.Domain.DTOs
{
    /// <summary>
    /// 学部评论
    /// </summary>
    public class SCMDto
    {
        public string No { get; set; }
        public Guid Id { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        public string HeadImgUrl { get; set; }


        /// <summary>
        /// 角色
        /// </summary>
        public Enums.SCMRole Role { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否过来人
        /// </summary>
        public bool IsExprience { get; set; }

        /// <summary>
        /// 是否学校辟谣
        /// </summary>
        public bool IsAuth { get; set; }

        /// <summary>
        /// 类型 0个人 1机构
        /// </summary>
        public int? TalentType { get; set; }

        /// <summary>
        /// 是否精华帖
        /// </summary>
        public bool IsEssence { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }


        /// <summary>
        /// 指示该点评是否已经被点赞
        /// </summary>
        public bool IsLike { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }


        /// <summary>
        /// 评论附加的图片
        /// </summary>
        public List<string> Imgs { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }


        /// <summary>
        /// 回复的数量
        /// </summary>
        public int ReplyCount { get; set; }


        /// <summary>
        /// 点赞数量
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校分部ID
        /// </summary>
        public Guid SchoolExtId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 学校分数
        /// </summary>
        public decimal SchoolScore { get; set; }

        /// <summary>
        /// 学校星星数
        /// </summary>
        public int SchoolStarts { get; set; }

        /// <summary>
        /// 学校点评数
        /// </summary>
        public int SchoolCommentCount { get; set; }
        /// <summary>
        /// 学校点评总平均分
        /// </summary>

        public decimal SchoolAvgScore { get; set; }


        public int SchoolNo { get; set; }
        public string ShortSchoolNo => ProductManagement.Framework.Foundation.UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();

        /// <summary>
        /// 点评评分
        /// </summary>
        public CommentScoreDto Score { get; set; }

        /// <summary>
        /// 点评评分
        /// </summary>
        public class CommentScoreDto
        {
            /// <summary>
            /// 点评ID
            /// </summary>
            /// <value>The report user identifier.</value>
            public Guid CommentId { get; set; }
            /// <summary>
            /// 是否就读
            /// </summary>
            public bool IsAttend { get; set; }
            /// <summary>
            /// 总分
            /// </summary>
            /// <value>The school score.</value>
            public decimal AggScore { get; set; }
            public decimal AggScoreStar => AggScore.ToStar();
            /// <summary>
            /// 师资力量分
            /// </summary>
            /// <value>The school score.</value>
            public decimal TeachScore { get; set; }
            public decimal TeachScoreStar => TeachScore.ToStar();
            /// <summary>
            /// 硬件设施分
            /// </summary>
            /// <value>The school score.</value>
            public decimal HardScore { get; set; }
            public decimal HardScoreStar => HardScore.ToStar();
            /// <summary>
            /// 环境周边分
            /// </summary>
            /// <value>The school score.</value>
            public decimal EnvirScore { get; set; }
            public decimal EnvirScoreStar => EnvirScore.ToStar();
            /// <summary>
            /// 学风管理分
            /// </summary>
            /// <value>The school score.</value>
            public decimal ManageScore { get; set; }
            public decimal ManageScoreStar => ManageScore.ToStar();
            /// <summary>
            /// 校园生活分
            /// </summary>
            /// <value>The school score.</value>
            public decimal LifeScore { get; set; }
            public decimal LifeScoreStar => LifeScore.ToStar();
        }
    }
}
