using System;
using System.Collections.Generic;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolCommentDto
    {
        public SchoolCommentDto()
        {
            CommentReplies = new List<CommentReplyDto>();
        }
        /// <summary>
        /// 序号
        /// </summary>
        public long No { get; set; }
        public string ShortCommentNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(No)?.ToLower();
            }
        }
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 写入点评的用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 学校id 
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 写入点评时勾选、新建的标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 写点评上传的图片集合，也需要取配置文件中的域名
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 点评评分
        /// </summary>
        public CommentScoreDto Score { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        //public string SchoolBranch { get; set; }

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

        ///// <summary>
        ///// 学校信息
        ///// </summary>
        public SchoolInfoDto SchoolInfo { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected => State == ExamineStatus.Highlight || ReplyCount >= 10;
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool IsRumorRefuting { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        public List<CommentReplyDto> CommentReplies { get; set; }

        public int? TalentType { get; set; }

        //public class CommentScore
        //{
        //    /// <summary>
        //    /// 是否就读
        //    /// </summary>
        //    public bool IsAttend { get; set; }

        //    /// <summary>
        //    /// 总分
        //    /// </summary>
        //    public decimal AggScore { get; set; }


        //    /// <summary>
        //    /// 师资力量分
        //    /// </summary>
        //    public decimal TeachScore { get; set; }

        //    /// <summary>
        //    /// 硬件设施分
        //    /// </summary>
        //    public decimal HardScore { get; set; }

        //    /// <summary>
        //    /// 环境周边分
        //    /// </summary>
        //    public decimal EnvirScore { get; set; }

        //    /// <summary>
        //    /// 学风管理分
        //    /// </summary>
        //    public decimal ManageScore { get; set; }

        //    /// <summary>
        //    /// 校园生活分
        //    /// </summary>
        //    public decimal LifeScore { get; set; }

        //}
    }



}
