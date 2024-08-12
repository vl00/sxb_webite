using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Sign
{
    public class ImportSchool
    {
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学校名称（学校名 +  分部名）
        /// </summary>
        public string Sname { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
    }
}
