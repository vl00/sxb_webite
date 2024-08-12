using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Models.ImportViewModels
{
    public class QuestionAndAnswer
    {
        /// <summary>
        /// 学校名
        /// </summary>
        [Description("学校名")]
        public string SchoolName { get; set; }

        /// <summary>
        /// 学部Id
        /// </summary>
        [Description("学部Id")]
        public Guid Eid { get; set; }

        /// <summary>
        /// 学部
        /// </summary>
        [Description("学部")]
        public string SchoolExtName { get; set; }

        /// <summary>
        /// 提问内容
        /// </summary>
        [Description("提问内容")]
        public string QuestionContent { get; set; }

        /// <summary>
        /// 回答内容
        /// </summary>
        [Description("回答内容")]
        public string AnswerContent { get; set; }

        /// <summary>
        /// 是否匿名发布[1:是 0:否]
        /// </summary>
        [Description("是否匿名发布[1:是 0:否]")]
        public bool IsAnony { get; set; }

        /// <summary>
        /// 是否为问答对比
        /// </summary>
        [Description("是否对比")]
        public bool IsContact { get; set; }

        public string SchoolNameFull { get { return SchoolName + SchoolNameFull; } }
    }
}
