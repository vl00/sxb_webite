using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Org
{
    public class HotSellCourse
    {
        public class HttpWrapper
        {
            public IEnumerable<HotSellCourse> Courses { get; set; }
        }
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string OrgName { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Subject { get; set; }
        public string Banner { get; set; }
        public string Price { get; set; }
        public string OrigPrice { get; set; }
        public bool Authentication { get; set; }

        /// <summary>
        /// 课程tag
        /// ["0-15岁","语文","低价体验"]
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
