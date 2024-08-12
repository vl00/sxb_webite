using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class openid_weixinDto
    {
        public string OpenId { get; set; }

        public Guid UserId { get; set; }

        public string AppName { get; set; }

        public DateTime? LastSubscribeTime { get; set; }
        public DateTime? LastUnSubscribeTime { get; set; }
    }
}

