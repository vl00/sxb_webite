using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.Dtos
{
   public  class SystemEventLogExportDataDto
    {

		public int Id { get; set; }

		/// <summary> 
		/// </summary> 
		public string AppName { get; set; }

		/// <summary> 
		/// </summary> 
		public string AppVersion { get; set; }

		/// <summary> 
		/// </summary> 
		public Guid? UserId { get; set; }

        public string NickName { get; set; }

        /// <summary> 
        /// 1->H5 2->PC 3->小程序 4->App 
        /// </summary> 
        public string Client { get; set; }

		/// <summary> 
		/// 设备型号 
		/// </summary> 
		public string Equipment { get; set; }

        public bool Login { get; set; }

        /// <summary> 
        /// 是否已认证达人 
        /// </summary> 
        public bool Talent { get; set; }

		/// <summary> 
		/// 系统型号 
		/// </summary> 
		public string System { get; set; }

		/// <summary> 
		/// </summary> 
		public string Location { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
        /// <summary> 
        /// </summary> 
        public string Event { get; set; }

		/// <summary> 
		/// </summary> 
		public string EventId { get; set; }

	
		/// <summary> 
		/// </summary> 
		public string Body { get; set; }

        public string Version { get; set; }


    }
}
