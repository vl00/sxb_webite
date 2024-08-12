using System;
namespace ProductManagement.UserCenter.TencentCommon.Model
{
    public class WXMiniProgramUserInfo
    {
        public string OpenId { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public string City { get; set; }
        public string province { get; set; }
        public string Country { get; set; }

        public string AvatarUrl { get; set; }
        public string UnionId { get; set; }
        public WatermarkClass Watermark { get; set; }

        public class WatermarkClass
        {
            public string Appid { get; set; }
            public long Timestamp { get; set; }
        }
    }
}
