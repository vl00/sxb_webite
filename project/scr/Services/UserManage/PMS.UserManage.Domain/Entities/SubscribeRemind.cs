using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    [Serializable]
    [Table("SubscribeRemind")]
    public class SubscribeRemind
    {
        [ExplicitKey]
		/// <summary> 
		/// 编号 
		/// </summary> 
		public Guid Id { get; set; }

		/// <summary> 
		/// 分组编码 
		/// </summary> 
		public string GroupCode { get; set; }

		/// <summary> 
		/// 用户id 
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary> 
		/// 订阅对象 
		/// </summary> 
		public Guid SubjectId { get; set; }

		/// <summary> 
		/// 是否有效 
		/// </summary> 
		public bool IsEnable { get; set; }

		/// <summary> 
		/// 是否删除 
		/// </summary> 
		public bool IsValid { get; set; }

		/// <summary> 
		/// 开始提醒时间 
		/// </summary> 
		public DateTime StartTime { get; set; }

		/// <summary> 
		/// 结束提醒时间 
		/// </summary> 
		public DateTime EndTime { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }
	}
}
