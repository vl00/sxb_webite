using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Dtos
{

    [Description("用户信息")]
    public class UserInfoDto
    {

        [Description("用户ID")]
        public Guid ID { get; set; }

        [Description("用户昵称")]
        public string NickName { get; set; }

        [Description("用户头像")]
        public string HeadImgUrl { get; set; }
    }
}
