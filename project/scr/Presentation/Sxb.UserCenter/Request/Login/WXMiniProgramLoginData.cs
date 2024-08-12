using System;
namespace Sxb.UserCenter.Request.Login
{
    public class WXMiniProgramLoginData
    {
        public string Code { get; set; }
        public string EncryptedData { get; set; }
        public string IV { get; set; }
        public string ReturnUrl { get; set; } = "";
        public string Mp { get; set; } = "sxkid";

        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
        public int Gender { get; set; }
    }
}
