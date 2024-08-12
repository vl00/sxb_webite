using System;
using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("TalentRecordDataRelation")]
	public partial class TalentRecordDataRelation
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid UserId {get;set;}

		 /// <summary> 
		 /// 1-文章 2->直播 3->案例 
		 /// </summary> 
		public Guid DataId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public TalentRecordDataRelationDataType DataType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Sort {get;set;}


	}
}