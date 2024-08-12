using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.SchoolRank
{
    public class SchoolRankCardViewModel
    {
        /// <summary>
        /// 榜单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 榜单名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 榜单背景图片链接
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// No ID的base32编码
        /// </summary>
        public string No { get; set; }
    }
}
