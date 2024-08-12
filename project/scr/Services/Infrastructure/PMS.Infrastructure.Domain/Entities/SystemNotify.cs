using System;
using Dapper.Contrib.Extensions;
using PMS.Infrastructure.Domain.Enums;

namespace PMS.Infrastructure.Domain.Entities
{
	[Serializable]
	[Table("SystemNotify")]
	public partial class SystemNotify
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 媒体类型，1-> 模态窗 
		 /// </summary> 
		public SystemNotifyMediaType MediaType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Body {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid FromUser {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid ToUser {get;set;}

		 /// <summary> 
		 /// 通知场景 0->任意场景， 1->达人引导上学问  2->未定义 4->未定义 
		 /// </summary> 
		public SystemNotifyInfoType InfoType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? ReadTime {get;set;}

		 /// <summary> 
		 /// 是否自动回应 
		 /// </summary> 
		public bool? IsAutoAsk {get;set;}


	}
}