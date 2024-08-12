using System;
namespace Sxb.UserCenter.Request.Login
{
    public class LoginData
    {
        public Guid Kid { get; set; }
        public short NationCode { get; set; } = 86;
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Rnd { get; set; }
        public string ReturnUrl { get; set; }
        public int? City { get; set; }
        public string Nickname { get; set; } = null;
        public string Fw { get; set; } = null;
        public int? Client { get; set; } = null;
    }
}
