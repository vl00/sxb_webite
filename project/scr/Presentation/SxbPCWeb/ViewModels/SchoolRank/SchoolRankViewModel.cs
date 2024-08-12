using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.SchoolRank
{
    public class SchoolRankViewModel
    {
        /// <summary>
        /// 榜单ID
        /// </summary>
        public Guid  RankId { get; set; }

        /// <summary>
        /// 榜单名称
        /// </summary>
        public string RankName { get; set; }


        /// <summary>
        /// 榜单背景图片链接
        /// </summary>
        public string RankCoverUrl { get; set; }

        public int No { get; set; }


        /// <summary>
        /// No ID的base32编码
        /// </summary>
        public string Base32No
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(this.No);
            }
        }

        /// <summary>
        /// 榜单旗下的学校信息
        /// </summary>

        public List<SchoolInfo> Schools { get; set; }


        public string Intro { get; set; }

        public string DTSource  { get; set; }

        public class SchoolInfo {

            public Guid Id { get; set; }

            public Guid ExtId { get; set; }

            public string Name { get; set; }

            public int Statu { get; set; }

            public int Sort { get; set; }

            public string Remark { get; set; }

            public int SchoolNo { get; set; }


        }
    }
}
