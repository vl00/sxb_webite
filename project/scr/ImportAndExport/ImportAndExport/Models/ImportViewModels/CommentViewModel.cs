using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Models.ImportViewModels
{
    public class CommentViewModel
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
        /// 学校名
        /// </summary>
        [Description("学校名")]
        public string SchoolName { get; set; }

        /// <summary>
        /// 分部ID
        /// </summary>
        [Description("学部Id")]
        public Guid Eid { get; set; }

        /// <summary>
        /// 学部名
        /// </summary>
        [Description("学部")]
        public string ExtName { get; set; }

        /// <summary>
        /// 写入者【用户池随机获取用户id】，写入者信息随机获取
        /// </summary>
        [Description("受邀达人Id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 回答者Id
        /// </summary>
        [Description("回答者Id")]
        public Guid RespondentId { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        [Description("点评内容")]
        public string CommentContent { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }

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

        public string SchoolNameFull { get { return SchoolName + ExtName; } }
    }
}
