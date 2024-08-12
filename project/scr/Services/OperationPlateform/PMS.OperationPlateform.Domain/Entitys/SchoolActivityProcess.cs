using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolActivityProcess")]
	public partial class SchoolActivityProcess
	{
		 /// <summary> 
		 /// </summary>
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid ActivityId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Title {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string StartTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string EndTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Description {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid CreatorId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid UpdatorId {get;set;}


	}
}