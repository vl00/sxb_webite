using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.Wx.Model
{
    public class WXMiniProgramAuth
    {
        public string OpenID { get; set; }
        public string Session_Key { get; set; }
        public string Unionid { get; set; }
        public int Errcode { get; set; }
        public string Errmsg { get; set; }
    }
}
