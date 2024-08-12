using PMS.UserManage.Domain.Entities;
using System;
namespace PMS.UserManage.Application.ModelDto
{
    public class UserInfoOpenIdDto
    {
        public Guid UserId { get; set; }

        public string OpenId { get; set; }

        public string AppId { get; set; }

        public string AppName { get; set; }
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }

        public string NickName { get; set; }

        public bool Blockage { get; set; }

        public string Mobile { get; set; }

        public string HeadImgUrl { get; set; }

        public int[] VerifyTypes { get; set; }

        public int? Sex { get; set; }

        public int? NationCode { get; set; }

        public int? City { get; set; }

        public string Channel { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime LoginTime { get; set; }

        public string Introduction { get; set; }

        public static implicit operator UserInfoDto(UserInfo userInfo)
        {
            if (userInfo == null) { return null; }
            return new UserInfoDto()
            {
                Id = userInfo.Id,
                Mobile = userInfo.Mobile,
                NickName = userInfo.NickName,
                HeadImgUrl = userInfo.HeadImgUrl ?? "https://cos.sxkid.com/images/school_v3/head.png",
                Blockage = userInfo.Blockage,
                Sex = userInfo.Sex,
                Channel = userInfo.Channel,
                City = userInfo.City,
                NationCode = userInfo.NationCode,
                LoginTime = userInfo.LoginTime,
                RegTime = userInfo.RegTime

            };
        }
    }

    public class InviteUserInfoDto : UserInfoDto
    {
        public bool isInvite { get; set; }

        public string intro1 { get; set; }

        public string intro2 { get; set; }
    }

    public class ThirdAuthUserDto : UserInfoDto 
    {
        public string AppName { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
    }

}
