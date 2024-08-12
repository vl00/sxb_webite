using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class InviteStatus
    {
		public int Id { get; set; }
		/// <summary>
		///2：邀请点评
		///3：邀请提问
		///4：邀请回答
		///100：点赞点评
		///101：点赞回答
		///102：点赞回复
		///103：新增粉丝
		/// </summary>
		public int Type { get; set; }
		/// <summary>
		/// 被邀请者
		/// </summary>
		public Guid UserId { get; set; }
		/// <summary>
		/// 邀请者
		/// </summary>
		public Guid SenderId { get; set; }
		/// <summary>
		/// 数据id
		/// </summary>
		public Guid DataId { get; set; }
		/// <summary>
		/// 邀请回答时 该字段存储问题内容
		/// </summary>
		public string Content { get; set; }
		/// <summary>
		/// 学部id
		/// </summary>
		public Guid Eid { get; set; }
		/// <summary>
		/// 邀请时间
		/// </summary>
		public DateTime SendTime { get; set; }
		/// <summary>
		/// 是否已读【true：未读，false：已读】
		/// </summary>
		public bool IsRead { get; set; }
	}
}
