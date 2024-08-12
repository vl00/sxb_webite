using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Enums;
using PMS.School.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Assess
{
    public class GetRportRequest
    {
        public Guid ID { get; set; }
    }
    public class GetRportResponse
    {
        public Dictionary<int, string> Questions { get; set; }
        public Dictionary<int, List<string>> Options { get; set; }
        public TalentDetailExtend RecommendTalent { get; set; }
        public IEnumerable<SchoolExtItemDto> RecommendSchools { get; set; }
        public AssessType AssessType { get; set; }
    }
}
