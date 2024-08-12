using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ResponseModel.ViewModels.User.Talent
{
    public class PageTalentItem
    {
        public DateTime ListDate { get; set; }
        public DateTime NextUpdateTime { get; set; }
        public IEnumerable<TalentItem> Items { get; set; }
    }

    public class TalentItem
    {
        public string TalentName { get; set; }
        public string HeadImgUrl { get; set; }
        public int Index { get; set; }
        public bool? IsUp { get; set; }
        public int Distance { get; set; }
        public Guid TalentUserID { get; set; }
        public Guid? LiveID { get; set; }
        public string TalentTitle { get; set; }
        public bool IsAttend { get; set; }
        public int Type { get; set; }
    }
}
