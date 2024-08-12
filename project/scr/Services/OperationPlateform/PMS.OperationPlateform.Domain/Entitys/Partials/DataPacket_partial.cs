using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	public partial class DataPacket
	{
		[Write(false)]
		public string OpenId { get; set; }
	}
}