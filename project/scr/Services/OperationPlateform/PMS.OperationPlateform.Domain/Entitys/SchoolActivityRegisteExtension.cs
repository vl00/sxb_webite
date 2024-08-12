using System;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolActivityRegisteExtension")]
	public partial class SchoolActivityRegisteExtension
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 报名Id 
		 /// </summary> 
		public Guid ActivityId {get;set;}

		 /// <summary> 
		 /// 字段 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// 字段值 
		 /// </summary> 
		public string Value {get;set;}

		 /// <summary> 
		 /// 字段类型：0、文本输入；1、手机号+验证码；2、下拉选择；3、单选；4、多选；5、多行文本； 
		 /// </summary> 
		public byte Type {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Version {get;set;}

		 /// <summary> 
		 /// 排序 
		 /// </summary> 
		public int Sort {get;set;}

		 /// <summary> 
		 /// 备注信息 
		 /// </summary> 
		public string Note {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// 创建时间 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// 创建人 
		 /// </summary> 
		public Guid? CreatorId {get;set;}


	}
}