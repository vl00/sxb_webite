using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.MessageViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public static class UserInfoDtoToVoHelper
    {
        public static UserInfoDetailModel UserDtoToVo(Talent user, FollowFansTotal fansTotal, PMS.UserManage.Application.ModelDto.Info.Interest interest,bool IsFollow) 
        {
            return new UserInfoDetailModel()
            {
                IsFollow = IsFollow,
                Id = user.Id,
                AuthTitle = user.Certification_preview,
                IsAuth = user.IsAuth,
                IsTalentStaff = user.IsTalentStaff,
                ParentUserId = user.ParentUserId,
                FansTotal = fansTotal.Fans,
                FollowTotal = fansTotal.Follow,
                UserName = user.Nickname,
                HeadImage = user.HeadImgUrl,
                Role = user.Role,
                Interest = interest,
                Introduction = user.Introduction,
                Organization_name = user.Organization_name,
                Eid = user.Eid
            };
        }


        public static MydynamicDetail UserDynamicDtoToVo(Talent user, MydynamicTotal dynamic, List<InviteMessageViewModel> Invites, List<FollowLive> live)
        {
            return new MydynamicDetail()
            {
                Id = user.Id,
                AuthTitle = user.Certification_preview,
                IsAuth = user.IsAuth,
                Follow = dynamic.Follow,
                History = dynamic.History,
                Total = dynamic.Total,
                UserName = user.Nickname,
                HeadImage = user.HeadImgUrl,
                Role = user.Role,
                Introduction = user.Introduction,
                Invites = Invites,
                FollowLive = live
            };
        }

    }
}
