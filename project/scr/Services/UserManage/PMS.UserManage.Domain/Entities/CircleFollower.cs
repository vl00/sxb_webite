using System;

namespace PMS.UserManage.Domain.Entities
{
	public partial class CircleFollower
	{
		/// <summary> 
		///  
		/// </summary> 
		public Guid Id { get; set; }

		/// <summary> 
		///  
		/// </summary> 
		public Guid? UserId { get; set; }

		/// <summary> 
		///  
		/// </summary> 
		public Guid? CircleId { get; set; }

		/// <summary> 
		/// 关注时间 
		/// </summary> 
		public DateTime? Time { get; set; }

		/// <summary> 
		///  
		/// </summary> 
		public DateTime? ModifyTime { get; set; }


	}
}