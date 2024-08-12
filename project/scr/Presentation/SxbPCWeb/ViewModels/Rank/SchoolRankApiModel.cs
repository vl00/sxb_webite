using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Rank
{
    public class SchoolRankApiModel
    {
        public Guid RankId { get; set; }

        public string RankName { get; set; }

        public int Sort { get; set; }

        public bool ToTop { get; set; }

        public string Cover { get; set; }

        public bool IsShow { get; set; }

        public int No { get; set; }
        public string Base32No
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(this.No);
            }
        }

        public IEnumerable<SchoolRankBindItems> Items { get; set; }

        public class SchoolRankBindItems
        {
            public Guid SchoolExtId { get; set; }

            public string SchoolName { get; set; }

            public int Sort { get; set; }

        }
    }

    public class SchoolRankList
    {
        public int Total { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public List<SchoolRankApiModel> Rows { get; set; }
    }

    public class SchoolRankData
    {
        public int ErrCode { get; set; }

        public string Msg { get; set; }

        public SchoolRankList Data { get; set; }

    }



}
