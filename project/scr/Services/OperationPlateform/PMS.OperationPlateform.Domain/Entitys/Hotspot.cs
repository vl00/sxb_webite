using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Hotspot")]
	public partial class Hotspot
	{
		[Key]  
		public Guid Id {get;set;}
		/// <summary> 
		/// ���� 
		/// </summary> 
		public string GroupName { get; set; }

		/// <summary> 
		/// �ؼ��� 
		/// </summary> 
		public string KeyName { get; set; }

		/// <summary> 
		/// ��ת���� 
		/// </summary> 
		public string LinkUrl { get; set; }

		/// <summary> 
		/// ������Id 
		/// </summary> 
		public Guid? DataId { get; set; }

		/// <summary> 
		/// Ͷ�ų��� 
		/// </summary> 
		public int CityId { get; set; }

		/// <summary> 
		/// Ͷ�ų��� 
		/// </summary> 
		public string CityName { get; set; }

		/// <summary> 
		/// ���� 
		/// </summary> 
		public int Sort { get; set; }

		/// <summary> 
		/// �ն����� 0 ��  1 PC 2 H5 4 APP 
		/// </summary> 
		public int ClientType { get; set; }

		/// <summary> 
		/// ״̬ 0 �� 1 ���� 
		/// </summary> 
		public int Status { get; set; }

		/// <summary> 
		/// �Ƿ�ɾ�� 
		/// </summary> 
		public bool IsDeleted { get; set; }

		/// <summary> 
		/// ����ʱ�� 
		/// </summary> 
		public DateTime CreateTime { get; set; }

		/// <summary> 
		/// ������ 
		/// </summary> 
		public string CreaterUserName { get; set; }
	}
}