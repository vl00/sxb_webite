using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("mobile_attribution")]
	public partial class mobile_attribution
	{
		[ExplicitKey]
		public string mobileHead {get;set;}

		public byte isp {get;set;}

		public string ispName {get;set;}

		public int? province {get;set;}

		public string provinceName {get;set;}

		public int? city {get;set;}

		public string cityName {get;set;}


	}
}