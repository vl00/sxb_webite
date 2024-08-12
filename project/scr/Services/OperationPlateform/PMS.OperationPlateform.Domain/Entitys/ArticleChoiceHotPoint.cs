using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ArticleChoiceHotPoint")]
	public partial class ArticleChoiceHotPoint
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int City {get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool IsShow {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? Creator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? UpdateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? Updator {get;set;}


	}
}