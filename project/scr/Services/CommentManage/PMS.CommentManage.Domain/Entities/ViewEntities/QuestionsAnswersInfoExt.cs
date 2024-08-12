using System;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
    public class QuestionsAnswersInfoExt
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 问答Id
        /// </summary>
        public Guid QuestionInfoId { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 父级UserId
        /// </summary>
        public Guid? ParentUserId { get; set; }

        public bool ParentUserIdIsAnony { get; set; }

        public ExamineStatus State { get; set; }

        public bool IsTop { get; set; }

        /// <summary>
        /// 问答写入者
        /// </summary>
        public Guid UserId { get; set; }

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
        /// 问答内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞总数
        /// </summary>

        public int LikeCount { get; set; }
        /// <summary>
        /// 回复总数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; set; }

        /// <summary>
        /// 问答写入日期
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
