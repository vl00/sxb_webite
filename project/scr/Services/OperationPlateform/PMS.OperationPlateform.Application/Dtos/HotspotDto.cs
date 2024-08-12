using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class HotspotDto
	{
		public Guid Id { get; set; }

		/// <summary> 
		/// 关键词 
		/// </summary> 
		public string KeyName { get; set; }

		/// <summary> 
		/// 跳转链接 
		/// </summary> 
		public string LinkUrl { get; set; }

		/// <summary> 
		/// 绑定数据Id 
		/// </summary> 
		public Guid? DataId { get; set; }
	}
}
