using ImportAndExport.Models;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class QuestionsAnswersInfo
    {
        public QuestionsAnswersInfo()
        {
            Id = Guid.NewGuid();
            State = 0;
            IsTop = false;
            LikeCount = 0;
            ReplyCount = 0;
            IsSettlement = false;
            IsContrast = false;
        }

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
        /// 问题审核状态（1：未阅，2：已阅，3：已加精，4：已屏蔽） 默认值为：1（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; set; }

        public bool IsTop { get; set; }

        /// <summary>
        /// 问答写入者
        /// </summary>
        public Guid UserId { get; set; }

        public UserRole PostUserRole { get; set; }

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

        /// <summary>
        /// 顶级父级id
        /// </summary>
        public Guid? FirstParentId { get; set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        public ImportType ImportType { get; set; }

        /// <summary>
        /// 是否为对比
        /// </summary>
        public bool IsContrast { get; set; }
    }
}
