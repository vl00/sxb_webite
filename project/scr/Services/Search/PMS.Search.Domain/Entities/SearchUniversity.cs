using System;
namespace PMS.Search.Domain.Entities
{
	public class SearchUniversity
	{
		/// <summary> 
		/// 编号 
		/// </summary> 
		public int Id { get; set; }

		/// <summary> 
		/// 学校名称 
		/// </summary> 
		public string Name { get; set; }

		/// <summary>
		/// 学校首字符
		/// </summary>
		public string FirstLetterName { get; set; }

		/// <summary> 
		/// 学校类型  1 其他 2 独立学院 4 双一流 8 211 16 985 
		/// </summary> 
		public int Type { get; set; }

		/// <summary> 
		/// 校徽
		/// </summary> 
		public string Logo { get; set; }

		/// <summary> 
		/// 所在省 
		/// </summary> 
		public int ProvinceId { get; set; }
		/// <summary> 
		/// 所在省 
		/// </summary> 
		public string ProvinceName { get; set; }

		/// <summary> 
		/// 所在市 
		/// </summary> 
		public int CityId { get; set; }
		/// <summary> 
		/// 所在市 
		/// </summary> 
		public string CityName { get; set; }

		/// <summary> 
		/// 招生代码 
		/// </summary> 
		public string EnrollmentCode { get; set; }

		/// <summary> 
		/// 关联达人UserId 
		/// </summary> 
		public Guid? UserId { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsDeleted { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }
	}
}
