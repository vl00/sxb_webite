using PMS.UserManage.Application.ModelDto.Info;
using Sxb.UserCenter.Models.MessageViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.UserInfoViewModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class MpEditUserVM
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }
    }
}
