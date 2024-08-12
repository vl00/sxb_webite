using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class ListTalentLevelsResponse
    {
        public IEnumerable<ListTalentLevelsItem> Items { get; set; }
    }

    public class ListTalentLevelsItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
