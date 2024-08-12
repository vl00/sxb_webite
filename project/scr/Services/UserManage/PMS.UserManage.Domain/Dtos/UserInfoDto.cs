using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class UserInfoDto
    {
        public string id { get; set; }
        public string nickname { get; set; }
        public int? sex { get; set; }
        public string messageId { get; set; }
        public string headImgUrl { get; set; }
        //简介
        public string Introduction { get; set; }
        //认证信息
        public string certificationmessage { get; set; }
    }

    public class RecommendUserDto
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }

        public bool IsSameCity { get; set; }
        public bool IsTalent { get; set; }
    }
}
