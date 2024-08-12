using PMS.UserManage.Application.ModelDto;
using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Utils
{
    public static class UserHelper
    {
        public static UserInfoVo ToAnonyUserName(this UserInfoVo userInfo, bool IsAnony)
        {
            if (userInfo == null)
            {
                userInfo = new UserInfoVo();
            }
            if (IsAnony)
            {
                userInfo.NickName = "匿名用户";
                //userInfo.HeadImager = userInfo.HeadImager;
                userInfo.HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png";
                userInfo.Role = new List<int>();
            }
            return userInfo;
        }

        public static List<UserInfoVo> UserDtoToVo(List<UserInfoDto> UserInfoDtos)
        {
            List<UserInfoVo> UserInfos = new List<UserInfoVo>();
            foreach (var item in UserInfoDtos)
            {
                if (item == null) continue;
                UserInfoVo User = new UserInfoVo();
                User.Id = item.Id;
                User.HeadImager = item.HeadImgUrl;
                User.Role = item.VerifyTypes.ToList();
                User.NickName = item.NickName;
                UserInfos.Add(User);
            }
            return UserInfos;
        }
    }
}
