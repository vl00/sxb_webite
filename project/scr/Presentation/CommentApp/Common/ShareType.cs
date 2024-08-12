using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Common
{
    public enum ShareType : int
    {
        [Description("点评")]
        Comment = 1,

        [Description("回复")]
        Reply = 2,

        [Description("提问")]
        Question = 3,

        [Description("回答")]
        Answer = 4,

        [Description("成功点评")]
        SuccessComment = 5,

        [Description("成功提问")]
        SuccessQuestion = 6
    }
}
