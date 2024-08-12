using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public class ConstantValue
    {
        /// <summary>
        /// 留资活动  学校默认图片
        /// </summary>
        public static string DefaultSchoolHeadImg { get; set; } = "https://cos.sxkid.com/images/schoolactivity/banner/7b4dd1d5-18cb-4add-b81f-742a8698e1ab.png";

        /// <summary>
        /// 留资活动 默认banner图
        /// </summary>
        //public string DefaultSchoolActivityBannerImg { get; set; } = "https://cos.sxkid.com/images/schoolactivity/banner/57960400-05ef-4991-82a1-a8c0e3920214.png";
        public static string DefaultSchoolActivityBannerImg { get; set; } = "https://cos.sxkid.com/images/schoolactivity/banner/74b0584e-59d6-4fab-9539-94c5f566e3aa.png";

        /// <summary>
        /// 密钥
        /// 对于HmacSha256Signature，秘密密钥长度不应小于128位；默认值为0。换句话说，它应该至少包含16个字符。
        /// </summary>
        public static string JwtKey { get; set; } = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB";
        /// <summary>
        /// 颁发给小程序使用
        /// </summary>
        public static string JwtAudience { get; set; } = "bilibili";
        /// <summary>
        /// 颁发者
        /// </summary>
        public static string JwtIssuer { get; set; } = "sxb-issuer";

        public static string DefRoleName { get; set; } = "普通用户";
    }
}
