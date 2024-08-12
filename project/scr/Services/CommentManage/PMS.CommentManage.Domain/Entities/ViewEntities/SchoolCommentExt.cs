using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 点评详情
    /// </summary>
    public class SchoolCommentExt
    {
        public long  No { get; set; }
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
        /// 点评审核状态
        /// </summary>
        public ExamineStatus State { get; set; }

        public UserRole PostUserRole { get; set; }

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
        /// 写入日期
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }
        /// <summary>
        /// 师资力量分
        /// </summary>
        /// <value>The school score.</value>
        public decimal TeachScore { get; set; }
        /// <summary>
        /// 硬件设施分
        /// </summary>
        /// <value>The school score.</value>
        public decimal HardScore { get; set; }
        /// <summary>
        /// 环境周边分
        /// </summary>
        /// <value>The school score.</value>
        public decimal EnvirScore { get; set; }
        /// <summary>
        /// 学风管理分
        /// </summary>
        /// <value>The school score.</value>
        public decimal ManageScore { get; set; }
        /// <summary>
        /// 校园生活分
        /// </summary>
        /// <value>The school score.</value>
        public decimal LifeScore { get; set; }

    }
}
