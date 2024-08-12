using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_SubscribePreference")]
	public partial class Article_SubscribePreference
	{
		[Key]  
		public int Id {get;set;}

		public Guid? UserId {get;set;}

		public int? ProvinceId {get;set;}

		public int? CityId {get;set;}

		public int? AreaId {get;set;}

		public string SchoolTypes {get;set;}

		public string Grades {get;set;}

		public bool? IsPushSubscibeSchool {get;set;}


	}
}