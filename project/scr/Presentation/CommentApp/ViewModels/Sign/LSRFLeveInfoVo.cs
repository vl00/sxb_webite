using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Sign
{
    public class LSRFLeveInfoVo
    {
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int City { get; set; }
        /// <summary>
        /// 留资类型 1：快捷，2：普通
        /// </summary>
        public int Type { get; set; }
        public int Area { get; set; }
        public int Stage { get; set; }
        public int CourseSetting { get; set; }
        public Guid? SchId { get; set; }
    }
}
