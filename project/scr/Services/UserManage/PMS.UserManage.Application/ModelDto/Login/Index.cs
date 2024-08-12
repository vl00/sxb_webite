using PMS.UserManage.Domain.Common;
using System;

namespace PMS.UserManage.Application.ModelDto.Login
{
    public class Get : RootModel
    {
        public string PublicKey { get; set; }
        public string Kid { get; set; }
        public string State { get; set; }

        public string RedirectUrl { get; set; }
        public bool IsLogin { get; set; }

        public string Login_Uri_WX { get; set; }
        public string Login_Uri_QQ { get; set; }
    }
    public class Post : RootModel
    {
        public Guid ID { get; set; }
        public string Nickname { get; set; }
        public string HeadImgUrl { get; set; }
        public bool MobileBinded { get; set; }
        /// <summary>
        /// 是否手机号重复的账号
        /// </summary>
        public bool MoblieRepeatUser { get; set; }
        /// <summary>
        /// 头像与昵称是否为默认
        /// </summary>
        public bool IsDefaultNicknameAndHeadImg
        {
            get
            {
                //var idHead = ID.ToString().Substring(0, 8);
                //var defaultNickName = $"手机用户-{idHead}";
                //if (string.IsNullOrWhiteSpace(Nickname) || string.IsNullOrWhiteSpace(HeadImgUrl)) return true;
                if (string.IsNullOrWhiteSpace(HeadImgUrl)) return true;
                //if (Nickname.ToLower() == defaultNickName && HeadImgUrl.ToLower().Contains("sxkid.com/images/head.png")) return true;
                if (HeadImgUrl.ToLower().Contains("sxkid.com/images/head.png")) return true;
                return false;
            }
        }
    }
}
