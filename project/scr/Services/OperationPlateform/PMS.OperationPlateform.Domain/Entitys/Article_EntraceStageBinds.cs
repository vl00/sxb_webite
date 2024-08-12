using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_EntraceStageBinds")]
	public partial class Article_EntraceStageBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public string EntraceStage {get;set;}


	}
}