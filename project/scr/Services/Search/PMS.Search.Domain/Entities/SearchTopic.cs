using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class SearchTopic
    {
		public List<SearchTag> Tags { get; set; }

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
		/// 谁可以看 
		/// </summary> 
		public Guid? OpenUserId { get; set; }

		/// <summary> 
		/// 类型  0 无 1 文本 2 图片 4 直播 8 文章 16 院校 32 点评 64 回答 128 外链 
		/// </summary> 
		public int Type { get; set; }

		/// <summary>
		/// 0：未屏蔽状态，1：屏蔽状态
		/// </summary>
		public int Status { get; set; }

		/// <summary> 
		/// 是否是问答话题 0 不是  1 是 
		/// </summary> 
		public byte IsQA { get; set; }

		/// <summary> 
		/// 置顶 0 无 1 圈主置顶  2 管理员置顶 
		/// </summary> 
		public int TopType { get; set; }

		/// <summary> 
		/// 置顶时间 
		/// </summary> 
		public DateTime? TopTime { get; set; }

		/// <summary> 
		/// 是否加精 
		/// </summary> 
		public byte IsGood { get; set; }

		/// <summary> 
		/// 加精时间 
		/// </summary> 
		public DateTime? GoodTime { get; set; }

		/// <summary> 
		/// 关注数量 
		/// </summary> 
		public long FollowCount { get; set; }

		/// <summary> 
		/// 点赞数量 
		/// </summary> 
		public long LikeCount { get; set; }

		/// <summary> 
		/// 回复数量 
		/// </summary> 
		public long ReplyCount { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public byte IsDeleted { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public Guid Creator { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary> 
		/// 最后回复时间 
		/// </summary> 
		public DateTime LastReplyTime { get; set; }

		/// <summary> 
		/// 最后编辑时间 
		/// </summary> 
		public DateTime LastEditTime { get; set; }

		/// <summary>
		/// 动态时间
		/// </summary>
		public DateTime Time { get; set; }


		public class SearchTag
		{
			public int Id { get; set; }
			public string Name { get; set; }

		}

	}
}
