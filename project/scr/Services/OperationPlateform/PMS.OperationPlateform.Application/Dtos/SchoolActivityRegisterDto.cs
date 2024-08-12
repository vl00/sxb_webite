using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class SchoolActivityRegisterDto 
    {

		/// <summary> 
		/// 活动id 
		/// </summary> 
		public Guid ActivityId { get; set; }

		/// <summary>
		/// 报名渠道
		/// </summary>
		public string Channel { get; set; }

		/// <summary>
		/// 统一留资必填
		/// </summary>
		public Guid? ExtId { get; set; }

		/// <summary> 
		/// 备注 
		/// </summary> 
		public string Note { get; set; }

		/// <summary> 
		/// 创建人 
		/// </summary> 
		public Guid? CreatorId { get; set; }

		/// <summary>
		/// 字段版本
		/// </summary>
		public int Version { get; set; }

		public List<Extension> Extensions { get; set; }


		/// <summary>
		/// 是否来源现场签到
		/// </summary>
		public bool IsSignInFromScene { get; set; } = false;

        public class Extension
        {
			public Guid Id { get; set; }

			public string Value { get; set; }

			/// <summary>
			/// 短信验证码
			/// </summary>
			public string SmsCode { get; set; }
		}

	}
}
