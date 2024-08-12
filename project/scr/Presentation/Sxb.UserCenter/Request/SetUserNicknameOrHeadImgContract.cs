namespace Sxb.UserCenter.Request
{
    public class SetUserNicknameOrHeadImgRequest
    {
        /// <summary>
        /// 不传则拉取微信昵称
        /// </summary>
        public string NickName { get; set; }
        public string OpenID { get; set; }
    }
}
