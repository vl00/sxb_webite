using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("article_cover")]
	public partial class article_cover
	{
		public Guid? photoID {get;set;}

		public Guid? articleID {get;set;}

		public byte sortID {get;set;}

		public byte ext {get;set;}

		public string ImgUrl { get;set; }
	}
}