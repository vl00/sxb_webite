using System;
namespace Sxb.UserCenter.Request.Login
{
    public class WXEncryptedData
    {
        public string EncryptedData { get; set; }
        public string IV { get; set; }

        public string Mp { get; set; } = "sxkid";
    }
}
