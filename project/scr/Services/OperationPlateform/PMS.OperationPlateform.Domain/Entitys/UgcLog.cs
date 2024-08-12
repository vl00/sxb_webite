using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{

	public partial class UgcLog
	{
		/// <summary> 
		/// </summary> 
		[ExplicitKey]
		public Guid Id { get; set; }

		/// <summary> 
		/// 推送用户 
		/// </summary> 
		public Guid UserId { get; set; }

		/// <summary> 
		/// 参见MessageDataType 
		/// </summary> 
		public int DataType { get; set; }

		/// <summary> 
		/// 对应DataType的id 
		/// </summary> 
		public Guid DataId { get; set; }

		/// <summary> 
		/// 推送城市 
		/// </summary> 
		public int AreaId { get; set; }

		/// <summary> 
		/// 类型  0 城市  1 城区 
		/// </summary> 
		public int AreaType { get; set; }

		/// <summary> 
		/// 创建时间 
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary> 
		/// 推送时间 
		/// </summary> 
		public DateTime? PushTime { get; set; }

		/// <summary> 
		/// 点击回馈时间 
		/// </summary> 
		public DateTime? TriggerTime { get; set; }

		/// <summary> 
		/// 点击回馈次序 
		/// </summary> 
		public int TriggerTimes { get; set; }

		/// <summary> 
		/// 内容相关学校 
		/// </summary> 
		public Guid ExtId { get; set; }

		/// <summary> 
		/// 发送的设备 
		/// </summary> 
		public string IosDeviceToken { get; set; }
		public string AndroidDeviceToken { get; set; }

		public int Status { get; set; }
		public string SchoolName { get; set; }
		public string Content { get; set; }
		public string BatchCode { get; set; }
		public long No { get; set; }
	}
}
