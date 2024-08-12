using PMS.UserManage.Application.ModelDto;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils
{
    public static class UserHelper
    {
        public static UserViewModel ToAnonyUserName(this UserViewModel userInfo, bool IsAnony)
        {
            if (IsAnony)
            {
                userInfo.UserName = "匿名用户";
                userInfo.HeadImage = "https://cos.sxkid.com/images/AppComment/defaultUserHeadImage.png";
            }
            return userInfo;
        }

        public static List<UserViewModel> UserDtoToVo(List<UserInfoDto> UserInfoDtos)
        {
            List<UserViewModel> UserInfos = new List<UserViewModel>();
            

            foreach (var item in UserInfoDtos)
            {


                if (item == null) continue;

                int? role = null;

                if (item.VerifyTypes != null) 
                {
                    if(item.VerifyTypes.Length > 0) 
                    {
                        role = item.VerifyTypes[0];
                    }
                }

                UserViewModel User = new UserViewModel();
                User.Id = item.Id;
                User.HeadImage = item.HeadImgUrl;
                User.Role = role;
                User.UserName = item.NickName;
                UserInfos.Add(User);
            }
            return UserInfos;
        }

    }
}
