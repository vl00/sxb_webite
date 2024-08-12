using System;
namespace Sxb.UserCenter.Request
{
    public class WxAccountData
    {
        public string OpenID { get; set; }
        public string Unionid { get; set; }
        public string Nickname { get; set; }
        public string HeadImgUrl { get; set; }
        public int Gender { get; set; }
    }
}
