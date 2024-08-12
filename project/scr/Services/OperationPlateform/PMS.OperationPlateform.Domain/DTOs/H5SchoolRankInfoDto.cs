using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public  class H5SchoolRankInfoDto
    {
        public Guid RankId { get; set; }

        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 榜单标题名称
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 榜单详情链接
        /// </summary>
        public string rank_url { get; set; }

        /// <summary>
        /// 排行榜中学部的名词
        /// </summary>
        public int Ranking { get; set; }

        public string Cover { get; set; }

        /// <summary>
        /// 上榜次数
        /// </summary>
        public int Count { get; set; }

        public int No { get; set; }

        public string Base32No
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(this.No);
            }
        }


    }
}
