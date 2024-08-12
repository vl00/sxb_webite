using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.ViewComponent
{
    public class PaidQAViewModel
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid TalentUserID { get; set; }
        /// <summary>
        /// 擅长领域
        /// </summary>
        public IEnumerable<string> RegionNames { get; set; }
        /// <summary>
        /// 提问内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }
    }
}
