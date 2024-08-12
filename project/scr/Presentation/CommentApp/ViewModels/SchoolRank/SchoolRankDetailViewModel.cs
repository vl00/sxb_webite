using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.SchoolRank
{
    public class SchoolRankDetailViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Cover { get; set; }

        public int? Area { get; set; }

        public decimal Rank { get; set; }

        public bool? ToTop { get; set; }

        public string Remark { get; set; }

        public bool? IsShow { get; set; }

        public string Intro { get; set; }

        public string DTSource { get; set; }

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



    }
}
