using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SysLog")]
	public partial class SysLog
	{
		[Key]  
		public int Id {get;set;}

		public string UserId {get;set;}

		public string Level {get;set;}

		public string ThreadId {get;set;}

		public string ClientIP {get;set;}

		public string HostIP {get;set;}

		public string Message {get;set;}

		public string HttpForm {get;set;}

		public string HttpCookie {get;set;}

		public string HttpQueryString {get;set;}

		public string Exception {get;set;}

		public string OriginUrl {get;set;}

		public string Url {get;set;}

		public string Controller {get;set;}

		public string Action {get;set;}

		public string Creator {get;set;}

		public string CreateTime {get;set;}


	}
}