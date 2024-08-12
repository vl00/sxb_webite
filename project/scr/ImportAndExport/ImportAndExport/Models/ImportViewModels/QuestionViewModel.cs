using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Models.ImportViewModels
{
    public class QuestionViewModel
    {
        /// <summary>
        /// 邀请日期
        /// </summary>
        [Description("邀请日期")]
        public DateTime InviteTime { get; set; }

        /// <summary>
        /// 学校ID
        /// </summary>
        [Description("学校Id")]
        public Guid Sid { get; set; }

        /// <summary>
        /// 分部ID
        /// </summary>
        [Description("学部Id")]
        public Guid Eid { get; set; }

        /// <summary>
        /// 写入者【用户池随机获取用户id】，写入者信息随机获取
        /// </summary>
        [Description("受邀达人Id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 提问者Id
        /// </summary>
        [Description("提问者Id")]
        public Guid CommitQuestionUserId { get; set; }

        /// <summary>
        /// 提问内容
        /// </summary>
        [Description("提问内容")]
        public string CommentContent { get; set; }

        /// <summary>
        /// 是否入读
        /// </summary>
        [Description("提问内容")]
        public bool IsAttend { get; set; }

        /// <summary>
        /// 是否匿名发布
        /// </summary>
        [Description("是否匿名发布[1:是 0:否]")]
        public bool IsAnony { get; set; }
    }
}
