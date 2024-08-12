using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolActivityRegiste")]
	public partial class SchoolActivityRegiste
	{
		 /// <summary> 
		 /// 报名Id 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 活动id 
		 /// </summary> 
		public Guid ActivityId {get;set;}

		/// <summary>
		/// 渠道
		/// </summary>
		public string Channel {get;set; }
		public string Phone {get;set; }

		/// <summary> 
		/// 学校学部id 
		/// </summary> 
		public Guid? ExtId {get;set;}

		 /// <summary> 
		 /// 字段1 
		 /// </summary> 
		public string Field0 {get;set;}

		 /// <summary> 
		 /// 字段2 
		 /// </summary> 
		public string Field1 {get;set;}

		 /// <summary> 
		 /// 字段3 
		 /// </summary> 
		public string Field2 {get;set;}

		 /// <summary> 
		 /// 字段4 
		 /// </summary> 
		public string Field3 {get;set;}

		 /// <summary> 
		 /// 字段5 
		 /// </summary> 
		public string Field4 {get;set;}

		 /// <summary> 
		 /// 字段6 
		 /// </summary> 
		public string Field5 {get;set;}

		 /// <summary> 
		 /// 字段7 
		 /// </summary> 
		public string Field6 {get;set;}

		 /// <summary> 
		 /// 字段8 
		 /// </summary> 
		public string Field7 {get;set;}

		 /// <summary> 
		 /// 字段9 
		 /// </summary> 
		public string Field8 {get;set;}

		 /// <summary> 
		 /// 字段10 
		 /// </summary> 
		public string Field9 {get;set;}

		 /// <summary> 
		 /// 版本 
		 /// </summary> 
		public int Version {get;set;}

		 /// <summary> 
		 /// 状态 
		 /// </summary> 
		public byte Status {get;set;}

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
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// 创建人 
		 /// </summary> 
		public Guid? CreatorId {get;set;}

		 /// <summary> 
		 /// 最后修改时间 
		 /// </summary> 
		public DateTime? LastModifyTime {get;set;}

		 /// <summary> 
		 /// 修改人 
		 /// </summary> 
		public Guid? ModifierId {get;set;}


		/// <summary>
		/// 0->未签到 1->已签到 2->现场签到
		/// </summary>
		public int SignInType { get; set; }


    }
}