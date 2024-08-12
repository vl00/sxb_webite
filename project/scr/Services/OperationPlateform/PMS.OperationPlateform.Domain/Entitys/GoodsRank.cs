using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("GoodsRank")]
	public class GoodsRank
	{
		[Key]
		public int Id { get; set; }
		/// <summary> 
		/// 短id 
		/// </summary>
		public string sid { get; set; }
		/// <summary> 
		/// 课程名称 
		/// </summary>
		public string courseName { get; set; }
		/// <summary> 
		/// 排序数字 
		/// </summary>
		public int sortNum { get; set; }
		/// <summary> 
		/// 商品状态
		/// </summary>
		public int? status { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool? IsDeleted { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime? createTime { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public string author { get; set; }
	}

}