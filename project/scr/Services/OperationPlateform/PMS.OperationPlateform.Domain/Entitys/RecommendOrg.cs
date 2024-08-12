using System;
using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("RecommendOrg")]
	public partial class RecommendOrg
	{
		/// <summary> 
		/// 编号 
		/// </summary> 
		[Key]
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 绑定数据Id 
		 /// </summary> 
		public string DataIds {get;set;}

		 /// <summary> 
		 /// 数据类型 1 机构 2 课程 
		 /// </summary> 
		public RecommendOrgDataType DataType {get;set;}

		 /// <summary> 
		 /// 投放城市  五级优先 ( -1 广深成重)   广州 深圳 成都 重庆 
		 /// </summary> 
		public int? CityId {get;set;}

		 /// <summary> 
		 /// 行政区域  四季优先 0 全部 
		 /// </summary> 
		public string AreaIds {get;set;}

		 /// <summary> 
		 /// 学校Id  二级优先 
		 /// </summary> 
		public Guid? SId {get;set;}

		 /// <summary> 
		 /// 学部Id  一级优先 
		 /// </summary> 
		public string ExtIds {get;set;}

		 /// <summary> 
		 /// 学校类型  三级优先  0 全部 
		 /// </summary> 
		public string SchoolTypes {get;set;}

		 /// <summary> 
		 /// 开始时间 
		 /// </summary> 
		public DateTime StartTime {get;set;}

		 /// <summary> 
		 /// 结束时间 
		 /// </summary> 
		public DateTime EndTime {get;set;}

		 /// <summary> 
		 /// 状态 0 无 1 启用 
		 /// </summary> 
		public int Status {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public bool IsDeleted {get;set;}

		 /// <summary> 
		 /// 创建时间 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// 创建人 
		 /// </summary> 
		public string CreaterUserName {get;set;}

		 /// <summary> 
		 /// 排序类型 10 学部 20 学校 30 学校类型 40 区域 50 城市 
		 /// </summary> 
		public RecommendType RecommendType {get;set;}


	}
}