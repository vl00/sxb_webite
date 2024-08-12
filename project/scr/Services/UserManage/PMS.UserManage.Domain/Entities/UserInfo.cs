using System;
using System.Collections.Generic;
using PMS.UserManage.Domain.Common;

namespace PMS.UserManage.Domain.Entities
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public short? NationCode { get; set; }

        public string Mobile { get; set; }
        public Guid? Password { get; set; }

        public string NickName { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime LoginTime { get; set; }

        public bool Blockage { get; set; }

        public string HeadImgUrl { get; set; }

        public int? Sex { get; set; }

        public int? City { get; set; }

        public string Channel { get; set; }

        public string VerifyTypes { get; set; }
        public string Introduction { get; set; }
        /// <summary>
        /// 渠道来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 客户端来源
        /// </summary>
        public RegisterClientEnum Client { get; set; }
    }
    

    public class UserInvite : UserInfo
    {
        public bool IsInvite { get; set; }

        public string Intro1 { get; set; }

        public string Intro2 { get; set; }
    }

    public class ThirdAuthUser : UserInfo 
    {
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string OpneId { get; set; }
        public string UnionId { get; set; }
    }
}
