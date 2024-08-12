using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class SaveSettingRequest
    {
        public bool IsEnable { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<Guid> GradeIDs { get; set; }
        public IEnumerable<Guid> RegionTypeIDs { get; set; }
    }
}
