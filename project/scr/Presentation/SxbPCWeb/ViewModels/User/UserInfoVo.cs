using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Models.User
{
    public class UserInfoVo
    {
        public UserInfoVo()
        {
            Role = new List<int>();
        }

        public Guid Id { get; set; }
        public string NickName { get; set; }
        public List<int> Role { get; set; }
        public string HeadImager { get; set; }

        public bool IsSchool => Role.Contains((int)UserRole.School);
        public bool IsTalent => Role.Contains((int)UserRole.PersonTalent) || Role.Contains((int)UserRole.OrgTalent);
    }
}
