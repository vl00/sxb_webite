using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
	/// <summary>
	/// 置顶
	/// </summary>
    public class TopDto
    {
		/// <summary>
		/// 推广置顶
		/// </summary>
		public List<ToppingDto> Toppings { get; set; }

		/// <summary>
		/// 话题置顶
		/// </summary>
		public List<ToppingDto> Topics { get; set; }


		public class ToppingDto
		{
			public Guid Id { get; set; }

			/// <summary> 
			/// 置顶内容 
			/// </summary> 
			public string Title { get; set; }

			/// <summary> 
			/// 外链 
			/// </summary> 
			public string Url { get; set; }
		}
    }
}
