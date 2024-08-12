using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models.SchoolActivity
{
    public class SchoolActivityProcessResult
    {
		public Guid Id { get; set; }

		/// <summary> 
		/// </summary> 
		public Guid ActivityId { get; set; }

		/// <summary> 
		/// </summary> 
		public string Title { get; set; }

		/// <summary> 
		/// </summary> 
		public string StartTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string EndTime { get; set; }

		/// <summary> 
		/// </summary> 
		public string Description { get; set; }

		/// <summary> 
		/// </summary> 
		public int Sort { get; set; }



	}
}
