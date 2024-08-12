using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Evaluate
{
    public class CreateEvaluateRequest: WebContentRequest
    {

        public Guid OrderID { get; set; }

        [Description("内容")]
        public string Content { get; set; }

        [Description("评分")]
        public int Score { get; set; }

        [Description("标签ID")]
        public List<Guid> TagIds { get; set; }

        public override string GetContent()
        {
            return this.Content;
        }
    }
  

}
