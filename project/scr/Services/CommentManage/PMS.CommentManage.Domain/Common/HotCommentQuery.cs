using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public class HotCommentQuery
    {
        /// <summary>
        ///true：带有学校类型条件，false：查询全类型
        /// </summary>
        public bool Condition { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int City { get; set; }
        public int Grade { get; set; }
        public int Type { get; set; }
        public bool Discount { get; set; }
        public bool Diglossia { get; set; }
        public bool Chinese { get; set; }
    }
}
