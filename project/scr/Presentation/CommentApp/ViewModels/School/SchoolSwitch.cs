using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.School
{
    public class SchoolSwitch
    {
        /// <summary>
        /// 分部Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 分部名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        public string City { get; set; }
    }
}
