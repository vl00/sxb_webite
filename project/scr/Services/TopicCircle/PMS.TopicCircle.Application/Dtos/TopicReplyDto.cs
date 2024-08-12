using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TopicReplyDto
    {
		public Guid Id { get; set; }

		/// <summary> 
		/// 深度 
		/// </summary> 
		public int Depth { get; set; }

		/// <summary> 
		/// 回复内容 
		/// </summary> 
		public string Content { get; set; }

		/// <summary> 
		/// 点赞数量 
		/// </summary> 
		public long LikeCount { get; set; }

		/// <summary> 
		/// 话题Id 
		/// </summary> 
		public Guid TopicId { get; set; }

		public Guid? ParentId { get; set; }
		public Guid? FirstParentId { get; set; }
		/// <summary> 
		/// 回复的人Id
		/// </summary> 
		public Guid? ParentUserId { get; set; }
		/// <summary> 
		/// 回复的人Name
		/// </summary> 
		public string ParentUserName { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public Guid Creator { get; set; }
		public string CreatorName { get; set; }
		public string HeadImgUrl { get; set; }

		public List<TopicReplyDto> Children { get; set; }
        public int ChildrenTotal { get; set; }



        public bool Follow { get; set; }

		public bool Like { get; set; }

		public string Time { get; set; }

		/// <summary>
		/// 是否是登录人的话题
		/// </summary>
		public bool IsLoginUserOwner { get; set; }
	}
}
