using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.Wx.Model
{
    public class WXUserInfoResult
    {
        public string subscribe { get; set; }
        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public string scope { get; set; }
        public string unionid { get; set; }
        /// 用户昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 用户个人资料填写的省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 普通用户个人资料填写的城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 国家，如中国为CN
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string headimgurl { get; set; }
        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
        /// </summary>
        public string[] privilege { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误提示消息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 关注状态
        /// </summary>
        public bool OpenIdSubscribeStatus { get; set; }
        public bool ForceBind { get; set; }

        public string AppName { get; set; }
        public string AppId { get; set; }
    }
}
