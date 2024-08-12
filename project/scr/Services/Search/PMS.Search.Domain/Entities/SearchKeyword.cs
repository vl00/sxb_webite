using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
	/// <summary>
	/// 搜索候选词词库
	/// </summary>
	public class SearchKeyword
	{
		/// <summary> 
		/// 候选词
		/// </summary> 
		public string Keyword { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		public bool IsDeleted { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		public DateTime? UpdateTime { get; set; }

		/// <summary>
		/// 附加分
		/// </summary>
		public decimal ExtraPoint { get; set; }

		/// <summary>
		/// 结果数
		/// </summary>
		public long ResultNumber { get; set; }
	}


	public class SearchKeywordHighlight
	{
		/// <summary> 
		/// 候选词
		/// </summary> 
		public string Keyword { get; set; }

		/// <summary>
		/// 高亮候选词
		/// </summary>
		public string Highlight { get; set; }
	}
}
