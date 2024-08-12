using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto.ModelVo
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoModel
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public List<int> Role { get; set; }
        public string HeadImager { get; set; }

    }
}
