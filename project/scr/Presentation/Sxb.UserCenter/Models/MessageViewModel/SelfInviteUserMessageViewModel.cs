using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.MessageViewModel
{
    public class SelfInviteUserMessageViewModel
    {
        public Guid DataId { get; set; }
        public int InviteTotal { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public string SchoolName { get; set; }
        public bool IsNewData { get; set; }
        public List<UserViewModel> Users { get; set; }
    }
}
