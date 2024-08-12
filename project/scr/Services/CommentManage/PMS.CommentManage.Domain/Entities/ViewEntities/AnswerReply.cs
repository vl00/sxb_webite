using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
    public class AnswerReply
    {
		/// <summary>
		/// 学部id
		/// </summary>
		public Guid SchoolSectionId { get; set; }
		/// <summary>
		/// 问题内容
		/// </summary>
		public string QuestionContent { get; set; }
		/// <summary>
		/// 提问者
		/// </summary>
		public Guid QuestionUserId { get; set; }
		/// <summary>
		/// 问题id
		/// </summary>
		public Guid QuestionId { get; set; }
		/// <summary>
		/// 是否为匿名提问
		/// </summary>
		public bool QuestionIsAnony { get; set; }
		/// <summary>
		/// 回答内容
		/// </summary>
		public string AnswerContent { get; set; }
		/// <summary>
		/// 回答者
		/// </summary>
		public Guid AnswerUserId { get; set; }
		/// <summary>
		/// 回答id
		/// </summary>
		public Guid AnswerId { get; set; }
		/// <summary>
		/// 是否为匿名回答
		/// </summary>
		public bool AnswerIsAnony { get; set; }
		/// <summary>
		/// 回复内容
		/// </summary>
		public string ReplyContent { get; set; }
		/// <summary>
		/// 回复者
		/// </summary>
		public Guid ReplyUserId { get; set; }
		/// <summary>
		/// 回复id
		/// </summary>
		public Guid ReplyId { get; set; }
		/// <summary>
		/// 是否为匿名回答
		/// </summary>
		public bool ReplyIsAnony { get; set; }
		/// <summary>
		/// 1：回答，0：回答的回复
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
	}
}
