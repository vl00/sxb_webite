using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ArticleChoiceHotPointBind")]
	public partial class ArticleChoiceHotPointBind
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 热点ID 
		 /// </summary> 
		public Guid ACHPId {get;set;}

		 /// <summary> 
		 /// 文章ID 
		 /// </summary> 
		public Guid? AId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public decimal Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid Creator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid Updator {get;set;}


	}
}