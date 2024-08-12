using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class InviteComment
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
        /// 邀请者
        /// </summary>
        [Description("邀请者Id")]
        public Guid InviteUser { get; set; }

        /// <summary>
        /// 被邀请者
        /// </summary>
        [Description("受邀达人Id")]
        public Guid ReceiveUser { get; set; }

        /// <summary>
        /// 回答者Id
        /// </summary>
        [Description("回答者Id")]
        public Guid AnswerUser { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        [Description("点评内容")]
        public string Content { get; set; }

        /// <summary>
        /// 是否匿名发布[1:是 0:否]
        /// </summary>
        [Description("是否匿名发布[1:是 0:否]")]
        public bool IsAnony { get; set; }

        /// <summary>
        /// 是否入读
        /// </summary>
        [Description("是否入读[1:是 0:否][非入读五项评分可为空直接填写评分]")]
        public bool IsAttend { get; set; }

        /// <summary>
        /// 师资力量分
        /// </summary>
        /// <value>The school score.</value>
        [Description("师资力量[1-5星]")]
        public decimal TeachScore { get; set; }

        /// <summary>
        /// 硬件设施分
        /// </summary>
        /// <value>The school score.</value>
        [Description("硬件设施[1-5星]")]
        public decimal HardScore { get; set; }

        /// <summary>
        /// 环境周边分
        /// </summary>
        /// <value>The school score.</value>
        [Description("周边环境[1-5星]")]
        public decimal EnvirScore { get; set; }

        /// <summary>
        /// 学风管理分
        /// </summary>
        /// <value>The school score.</value>
        [Description("校风管理[1-5星]")]
        public decimal ManageScore { get; set; }

        /// <summary>
        /// 校园生活分
        /// </summary>
        /// <value>The school score.</value>
        [Description("校园活动[1-5星]")]
        public decimal LifeScore { get; set; }
    }
}
