using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    public class InviteSelf
    {
        public Guid DataId { get; set; }
        public int Type { get; set; }
        public int InviteTotal { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SchName { get; set; }
        public bool IsRead { get; set; }
    }

    public class InviteUserInfo
    {
        public Guid DataId { get; set; }
        public int Type { get; set; }
        public Guid UserId { get; set; }
        public string HeadImgUrl { get; set; }
    }

    //获赞
    public class DataMsg 
    {
        public int Type { get; set; }
        public int DataType { get; set; }
        public Guid DataId { get; set; }
        public string SName { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid DataUserId { get; set; }
        public Guid OUserId { get; set; }
        public string OName { get; set; }
        public string OImage { get; set; }
    }
}
