using ImportAndExport.Models;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class QuestionInfo
    {
        public QuestionInfo()
        {
            Id = Guid.NewGuid();
            State = ExamineStatus.Unread;
            LikeCount = 0;
            ReplyCount = 0;
            IsHaveImagers = false;
        }

        public Guid Id { get; set; }
        /// <summary>
        /// 问题审核状态（0：未阅，1：已阅，2：已加精，-1：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid UserId { get; set; }

        public UserRole PostUserRole { get; set; }

        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 导入类型
        /// </summary>
        public ImportType ImportType { get; set; }
        /// <summary>
        /// 是否为对比
        /// </summary>
        public bool IsContrast { get; set; }

        public List<QuestionsAnswersInfo> answer { get; set; }
    }
}
