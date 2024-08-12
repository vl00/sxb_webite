using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models.SystemEventLog
{
    public class AddLogRequest
    {
		/// <summary> 
		/// </summary> 
		public string AppName { get; set; }

		/// <summary> 
		/// </summary> 
		public string AppVersion { get; set; }

		/// <summary> 
		/// 设备型号 
		/// </summary> 
		public string Equipment { get; set; }

		/// <summary>
		/// 系统型号
		/// </summary>
        public string System { get; set; }


        /// <summary> 
        /// </summary> 
        public string Event { get; set; }

		/// <summary> 
		/// </summary> 
		public string EventId { get; set; }

		/// <summary> 
		/// </summary> 
		public string Body { get; set; }

		/// <summary> 
		/// </summary> 
		public string Creator { get; set; }

		public DateTime Time { get; set; } = DateTime.Now;

    }
}
