using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.ViewComponent
{
    public class PaidQAViewModel
    {
        public string NickName { get; set; }
        public Guid TalentUserID { get; set; }
        public IEnumerable<string> RegionNames { get; set; }
        public string Content { get; set; }
        public string HeadImgUrl { get; set; }
        public string AuthName { get; set; }
        /// <summary>
        /// 达人类型(0.个人 | 1.机构)
        /// </summary>
        public int TalentType { get; set; }
    }
}
