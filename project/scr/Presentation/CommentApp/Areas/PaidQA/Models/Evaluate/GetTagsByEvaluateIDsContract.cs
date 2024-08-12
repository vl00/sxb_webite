using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Evaluate
{
    public class GetTagsByEvaluateIDsRequest
    {
        public IEnumerable<Guid> IDs { get; set; }
    }
    public class GetTagsByEvaluateIDsResponse
    {
        public Dictionary<Guid, IEnumerable<EvaluateTags>> Tags { get; set; }
    }
}
