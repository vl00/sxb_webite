using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Inside.ViewModels.Comment
{
    public class HotComentQueryViewModel
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
