using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Hotspot")]
	public partial class Hotspot
	{
		[Key]  
		public Guid Id {get;set;}
		/// <summary> 
		/// 分组 
		/// </summary> 
		public string GroupName { get; set; }

		/// <summary> 
		/// 关键词 
		/// </summary> 
		public string KeyName { get; set; }

		/// <summary> 
		/// 跳转链接 
		/// </summary> 
		public string LinkUrl { get; set; }

		/// <summary> 
		/// 绑定数据Id 
		/// </summary> 
		public Guid? DataId { get; set; }

		/// <summary> 
		/// 投放城市 
		/// </summary> 
		public int CityId { get; set; }

		/// <summary> 
		/// 投放城市 
		/// </summary> 
		public string CityName { get; set; }

		/// <summary> 
		/// 排序 
		/// </summary> 
		public int Sort { get; set; }

		/// <summary> 
		/// 终端类型 0 无  1 PC 2 H5 4 APP 
		/// </summary> 
		public int ClientType { get; set; }

		/// <summary> 
		/// 状态 0 无 1 启用 
		/// </summary> 
		public int Status { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsDeleted { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public string CreaterUserName { get; set; }
	}
}