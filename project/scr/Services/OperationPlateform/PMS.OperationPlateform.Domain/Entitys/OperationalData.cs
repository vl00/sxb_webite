using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("OperationalData")]
	public partial class OperationalData
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ActivityId {get;set;}

		public string ActivityName {get;set;}

		public string ChannelName {get;set;}

		public int? PV {get;set;}

		public int? UV {get;set;}

		public int? IP {get;set;}

		public int? TakePartCount {get;set;}

		public int? ShareCount {get;set;}

		public int? ExchangeCount {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}