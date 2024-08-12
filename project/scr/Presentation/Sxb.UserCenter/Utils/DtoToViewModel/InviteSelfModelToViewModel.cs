using PMS.UserManage.Domain.Common;
using Sxb.UserCenter.Models.MessageViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public static class InviteSelfModelToViewModel
    {
        public static List<SelfInviteUserMessageViewModel> InviteSelfModelToViewModelHelper(List<InviteSelf> inviteUsers,List<InviteUserInfo> userInfos) 
        {
            List<SelfInviteUserMessageViewModel> model = new List<SelfInviteUserMessageViewModel>();
            foreach (var item in inviteUsers)
            {
                var userVo = userInfos.Where(x => x.DataId == item.DataId && x.Type == item.Type).Select(x => new UserViewModel() { HeadImage = x.HeadImgUrl }).ToList();
                SelfInviteUserMessageViewModel temp = new SelfInviteUserMessageViewModel() 
                {
                    DataId = item.DataId,
                    SchoolName = item.SchName,
                    Content = item.Content,
                    IsNewData = !item.IsRead,
                    Type = item.Type,
                    Users = userVo
                };

                model.Add(temp);
            }
            return model;
        }

    }
}
