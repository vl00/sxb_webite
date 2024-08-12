using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SearchSchoolRank : SearchPUV
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<SchoolRankItem> Schools { get; set; }
        public List<int> CityCodes { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SchoolRankItem
    {
        public int RankNo { get; set; }
        public string Name { get; set; }
    }

}
