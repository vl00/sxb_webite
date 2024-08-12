using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.ViewDto.SchoolVo
{
    /// <summary>
    /// 学校信息实体
    /// </summary>
    public class SchoolModel
    {
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
    }
}
