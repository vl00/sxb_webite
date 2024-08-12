using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
	/// <summary>
	/// 获赞
	/// </summary>
    public class ParentReply
    {
		/// <summary>
		/// 学部id
		/// </summary>
		public Guid SchoolSectionId { get; set; }
		/// <summary>
		/// 点评内容
		/// </summary>
		public string CommentContent { get; set; }
		/// <summary>
		/// 点评用户
		/// </summary>
		public Guid CommentUserId { get; set; }
		/// <summary>
		/// 点评id
		/// </summary>
		public Guid CommentId { get; set; }
		/// <summary>
		/// 回复内容
		/// </summary>
		public string ParentReplyContent { get; set; }
		/// <summary>
		/// 回复用户
		/// </summary>
		public Guid ParentUserId { get; set; }
		/// <summary>
		/// 回复id
		/// </summary>
		public Guid ParentId { get; set; }
		/// <summary>
		/// 回复回复内容
		/// </summary>
		public string ReplyContent { get; set; }
		/// <summary>
		/// 回复回复用户
		/// </summary>
		public Guid ReplyUserId { get; set; }
		/// <summary>
		/// 回复回复id
		/// </summary>
		public Guid ReplyId { get; set; }
		/// <summary>
		/// 1:回复，0：回复的回复
		/// </summary>
		public int Type { get; set; }

		/// <summary>
        /// 点赞数
        /// </summary>
		public int LikeCount { get; set; }
		/// <summary>
		/// 回复数
		/// </summary>
		public int ReplyCount { get; set; }
		/// <summary>
		/// 是否匿名点评
		/// </summary>
		public bool CommentIsAnony { get; set; }
		/// <summary>
		/// 匿名回复
		/// </summary>
		public bool ParentIsAnony { get; set; }
		/// <summary>
		/// 匿名回复
		/// </summary>
		public bool ReplyIsAnony { get; set; }
	}
}
