using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class SearchTalentDynamic
    {
        public int Type { get; set; }
        public long Total { get; set; }
        public List<TalentDynamicItem> TalentDynamicItems { get; set; }
    }

    public class TalentDynamicItem 
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }
}
