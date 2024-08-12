using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Application.ModelDto
{
    public class SearchTopicDto
	{

		public Guid Id { get; set; }

		/// <summary> 
		/// 话题圈 
		/// </summary> 
		public Guid CircleId { get; set; }

		/// <summary> 
		/// 话题内容 
		/// </summary> 
		public string Content { get; set; }

		/// <summary> 
		/// 最后回复时间 
		/// </summary> 
		public DateTime LastReplyTime { get; set; }
	}
}
