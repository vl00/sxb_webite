using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{
    /// <summary>
    /// 分页model
    /// </summary>
    public class PaginationReqDto
    {
        private int pageIndex = 1;
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageIndex
        {
            get => pageIndex;
            set => pageIndex = value > 0 ? value : 1;
        }

        private int pageSize = 10;
        /// <summary>
        /// 分页页码
        /// </summary>
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > 0 ? value : 10;
        }
    }
}
