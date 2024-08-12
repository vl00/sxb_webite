using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.UserManage.Domain.Common;

namespace Sxb.Api.ViewDto.UserInfoVo
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoModel
    {
        //private string QueryImage { get; set; } = "";
       
        public UserInfoModel()
        {
        }

        public Guid Id { get; set; }
        public string NickName { get; set; }
        public List<int> Role { get; set; }
        public string HeadImager { get; set; }

        public bool IsSchool => Role.Contains((int)UserRole.School);
    }
}
