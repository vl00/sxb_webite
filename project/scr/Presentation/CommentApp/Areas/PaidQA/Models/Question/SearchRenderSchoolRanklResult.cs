using Newtonsoft.Json;
using ProductManagement.Framework.AspNetCoreHelper.Utils;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models
{
    public class SearchRenderSchoolRanklResult
    {

		public Guid Id { get; set; }

		public string Title { get; set; }

		public string Cover { get; set; }

		public int? Area { get; set; }

		public decimal Rank { get; set; }

		public bool? ToTop { get; set; }

		public string Remark { get; set; }

		public bool? IsShow { get; set; }

		[JsonConverter(typeof(HmDateTimeConverter))]
		public DateTime? CreateTime { get; set; }

		public string Creator { get; set; }

		[JsonConverter(typeof(HmDateTimeConverter))]
		public DateTime? UpdateTime { get; set; }

		public string Updator { get; set; }

		public bool IsDel { get; set; }

		public string Intro { get; set; }

		public string DTSource { get; set; }
		public int No { get; set; }
		public string ShortId
		{
			get
			{
				return UrlShortIdUtil.Long2Base32(No).ToLower();
			}
		}
	}
}
