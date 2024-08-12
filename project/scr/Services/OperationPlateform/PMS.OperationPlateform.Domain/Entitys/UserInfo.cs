using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UserInfo")]
	public partial class UserInfo
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public string email {get;set;}

		public string mobile {get;set;}

		public string password {get;set;}

		public string token {get;set;}

		public string nickname {get;set;}

		public byte sex {get;set;}

		public int? city {get;set;}

		public string address {get;set;}

		public DateTime? childBirth {get;set;}

		public byte childSex {get;set;}

		public bool? headImg {get;set;}

		public string childSchool {get;set;}

		public byte childGrade {get;set;}

		public byte childClass {get;set;}

		public byte level {get;set;}

		public DateTime? regTime {get;set;}

		public DateTime? loginTime {get;set;}

		public DateTime? activeTime {get;set;}

		public bool? emailConfirm {get;set;}

		public long? ipAddress {get;set;}

		public bool? blockage {get;set;}

		public string openid_qq {get;set;}

		public long? uid_weibo {get;set;}

		public string userid_alipay {get;set;}

		public string unionid_weixin {get;set;}

		public string openid_weixin_mp {get;set;}

		public string openid_weixin_mp_fwh {get;set;}

		public string openid_weixin_mp_gz {get;set;}

		public string openid_weixin_mp_cd {get;set;}

		public string openid_weixin_mp_sz {get;set;}

		public string openid_weixin_mp_fs {get;set;}

		public string openid_weixin_app {get;set;}

		public string openid_weixin_mini {get;set;}

		public string openid_weixin_mini_lecture {get;set;}

		public string nickname_weixin {get;set;}

		public double latitude {get;set;}

		public double longitude {get;set;}

		public string deviceToken_ios {get;set;}

		public string deviceToken_android {get;set;}

		public string headImgUrl {get;set;}

		public string channel {get;set;}

		public string headImgUrl_tp {get;set;}


	}
}