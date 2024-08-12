using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolActivityExtension")]
	public partial class SchoolActivityExtension
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 活动id 
		 /// </summary> 
		public Guid ActivityId {get;set;}

		 /// <summary> 
		 /// 字段名称 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// 字段内容 
		 /// </summary> 
		public string Value {get;set;}

		 /// <summary> 
		 /// 存放缩略图 
		 /// </summary> 
		public string Value2 {get;set;}

		 /// <summary> 
		 /// 排序 
		 /// </summary> 
		public byte Sort {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// 备注 
		 /// </summary> 
		public string Note {get;set;}

		 /// <summary> 
		 /// 创建时间 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// 创建人 
		 /// </summary> 
		public Guid CreatorId {get;set;}

		 /// <summary> 
		 /// 修改时间 
		 /// </summary> 
		public DateTime? LastModifyTime {get;set;}

		 /// <summary> 
		 /// 修改人 
		 /// </summary> 
		public Guid? ModifierId {get;set;}


	}
}