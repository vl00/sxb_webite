using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Assess
{
    public class UpdateRequest
    {
        public int QuestionLevel { get; set; }
        public IEnumerable<Guid> OptionIDs { get; set; }

        public string Content { get; set; }
        public AssessSelectType SelectType { get; set; }
    }
}
