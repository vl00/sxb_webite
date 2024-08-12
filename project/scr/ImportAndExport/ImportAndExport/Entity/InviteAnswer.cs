using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class InviteAnswer
    {
        /// <summary>
        /// 邀请时间
        /// </summary>
        [Description("邀请日期")]
        public DateTime InviteTime { get; set; }

        /// <summary>
        /// 达人名称
        /// </summary>
        [Description("达人名称")]
        public string DarenName { get; set; }

        /// <summary>
        /// 学校id
        /// </summary>
        [Description("学校Id")]
        public Guid Sid { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [Description("学校名")]
        public string Sname { get; set; }

        /// <summary>
        /// 分部id
        /// </summary>
        [Description("学部Id")]
        public Guid Eid { get; set; }

        /// <summary>
        /// 分部名称
        /// </summary>
        [Description("学部")]
        public string Ename { get; set; }

        /// <summary>
        /// 问题id
        /// </summary>
        [Description("问题Id")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 邀请者
        /// </summary>
        [Description("邀请者Id")]
        public Guid InviteUser { get; set; }

        /// <summary>
        /// 被邀请者
        /// </summary>
        [Description("受邀达人Id")]
        public Guid ReceiveUser { get; set; }

        
        [Description("回答者Id")]
        public Guid AnswerUser { get; set; }
        
        /// <summary>
        /// 问题内容
        /// </summary>
        [Description("问题内容")]
        public string Content { get; set; }

        /// <summary>
        /// 是否匿名发布[1:是 0:否]
        /// </summary>
        [Description("回答内容")]
        public string AnswerContent { get; set; }

        /// <summary>
        /// 是否匿名发布[1:是 0:否]
        /// </summary>
        [Description("是否匿名发布[1:是 0:否]")]
        public bool IsAnony { get; set; }

        /// <summary>
        /// 是否过来人发布
        /// </summary>
        [Description("是否为过来人发布[1:是 0:否]")]
        public bool IsAttend { get; set; }
    }
}
