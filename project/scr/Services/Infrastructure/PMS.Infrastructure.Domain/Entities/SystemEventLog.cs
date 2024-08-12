using System;
using Dapper.Contrib.Extensions;

namespace PMS.Infrastructure.Domain.Entities
{
	[Serializable]
	[Table("SystemEventLog")]
	public partial class SystemEventLog
	{
		 /// <summary> 
		 /// </summary> 
		 [Key]
		public int Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string AppName {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string AppVersion {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? UserId {get;set;}
		public Guid? DeviceId { get; set; }

		/// <summary> 
		/// 1->H5 2->PC 3->小程序 4->App 
		/// </summary> 
		public int? Client {get;set;}

		 /// <summary> 
		 /// 设备型号 
		 /// </summary> 
		public string Equipment {get;set;}

		 /// <summary> 
		 /// 是否已认证达人 
		 /// </summary> 
		public bool? IsTalent {get;set;}

		 /// <summary> 
		 /// 系统型号 
		 /// </summary> 
		public string System {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Location {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Event {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string EventId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Body {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Remark {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Creator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}


	}
}