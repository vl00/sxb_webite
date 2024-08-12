using PMS.UserManage.Application.ModelDto;
using Sxb.PCWeb.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.PCWeb.Utils
{
    public static class UserHelper
    {
        public static UserInfoVo ToAnonyUserName(this UserInfoVo userInfo, bool IsAnony)
        {
            if (IsAnony)
            {
                if (userInfo == null) userInfo = new UserInfoVo();
                userInfo.NickName = "匿名用户";
                userInfo.HeadImager = "https://cos.sxkid.com/images/AppComment/defaultUserHeadImage.png";
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
