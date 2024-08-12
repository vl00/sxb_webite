using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Common
{
    public enum ReportTypeEnum : int
    {
        [Description("点评举报")]
        Comment = 1,

        [Description("问题举报")]
        Question = 2,

        [Description("点评回复举报")]
        CommentReplay = 3,

        [Description("回答举报")]
        Answer = 4,

    }
}
