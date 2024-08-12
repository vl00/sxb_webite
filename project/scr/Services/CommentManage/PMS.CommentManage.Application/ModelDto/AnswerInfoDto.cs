using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class AnswerInfoDto
    {
        /// <summary>
        /// 回答id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 该回答的点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回答的回复数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Reply { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前回复是否为精华
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerContent { get; set; }
        public string AddTime { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }

        /// <summary>
        /// 问题id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }

        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ParentUserId { get; set; }
        public bool ParentUserIdIsAnony { get; set; }

        public QuestionDto questionDto { get; set; }
    }
}
