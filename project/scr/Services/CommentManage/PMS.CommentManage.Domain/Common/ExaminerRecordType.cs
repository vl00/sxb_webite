using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{ 

    public enum ExaminerRecordType : int
    {
        /// <summary>
        /// 点评
        /// </summary>
        [Description("点评")]
        Comment = 1,
        
        /// <summary>
        /// 问答
        /// </summary>
        [Description("问答")]
        Answer = 2,
        
        /// <summary>
        /// 问题
        /// </summary>
        [Description("问题")]
        Question = 3,

        /// <summary>
        /// 结算
        /// </summary>
        [Description("结算")]
        Settlement = 4,
    }
}
