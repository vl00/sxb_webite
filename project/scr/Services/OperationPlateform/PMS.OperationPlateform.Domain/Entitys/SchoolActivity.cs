using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolActivity")]
	public partial class SchoolActivity
	{
		 /// <summary> 
		 /// 活动id 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 主标题 
		 /// </summary> 
		public string Title {get;set;}

		 /// <summary> 
		 /// 副标题 
		 /// </summary> 
		public string Subtitle {get;set;}

		 /// <summary> 
		 /// 学校学部id 
		 /// </summary> 
		public Guid? ExtId {get;set;}

		 /// <summary> 
		 /// 留资类型 
		 /// </summary> 
		public byte Type {get;set;}

		 /// <summary> 
		 /// 类别(单一或统一) 
		 /// </summary> 
		public byte Category {get;set;}

		 /// <summary> 
		 /// 活动名称 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// 是否关联学校 
		 /// </summary> 
		public bool IsConnectSchool {get;set;}

		 /// <summary> 
		 /// 活动起始时间 
		 /// </summary> 
		public DateTime? StartTime {get;set;}

		 /// <summary> 
		 /// 活动结束时间 
		 /// </summary> 
		public DateTime? EndTime {get;set;}

		 /// <summary> 
		 /// 状态 
		 /// </summary> 
		public byte Status {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public bool IsDeleted {get;set;}

		 /// <summary> 
		 /// 客户-金主 
		 /// </summary> 
		public string Customer {get;set;}

		 /// <summary> 
		 /// 是否替换学校详情内留资字段 
		 /// </summary> 
		public bool IsCover {get;set;}

		 /// <summary> 
		 /// 二维码地址 
		 /// </summary> 
		public string QRcodeUrl {get;set;}

		 /// <summary> 
		 /// 二维码标题 
		 /// </summary> 
		public string QRcodeTitle {get;set;}

		 /// <summary> 
		 /// 添加报名成功后说明 
		 /// </summary> 
		public string RegisteSuccessNote {get;set;}

		 /// <summary> 
		 /// 微信分享文案标题 
		 /// </summary> 
		public string WechatShareTitle {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string WechatShareIntro {get;set;}

		 /// <summary> 
		 /// 创建时间 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// 创建id 
		 /// </summary> 
		public Guid? CreatorId {get;set;}

		 /// <summary> 
		 /// 最后修改时间 
		 /// </summary> 
		public DateTime? LastModifyTime {get;set;}

		 /// <summary> 
		 /// 修改人 
		 /// </summary> 
		public Guid? ModifierId {get;set;}

		/// <summary>
		/// 是否x渠道限制同手机重复提交
		/// </summary>
		public bool IsUnique { get; set; }

        public string Description { get; set; }

    }
}