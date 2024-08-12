using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TagDetailDto
    {
		public int Id { get; set; }

		/// <summary> 
		/// 标签名称 
		/// </summary> 
		public string Name { get; set; }

		public List<TagDetailDto> Children { get; set; }
	}
}
