using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels
{
    /// <summary>
    /// 根据学校名称进行模糊筛选
    /// </summary>
    public class SchoolNameQuery
    {
        public Guid SchoolId { get; set; }
        public Guid School { get; set; }
        public string SchoolName { get; set; }
        public int CurrentTotal { get; set; }
    }
}
