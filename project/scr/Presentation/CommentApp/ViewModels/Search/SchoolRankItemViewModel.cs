using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.Search
{
    public class SchoolRankItemViewModel
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        /// <summary>
        /// 榜单ID
        /// </summary>
        public Guid Id { get; set; }

		/// <summary>
		/// 榜单名称
		/// </summary>
		public string RankName { get; set; }

        /// <summary>
        /// 榜单旗下的学校信息
        /// </summary>
        public string Schools { get; set; }
    }
}
