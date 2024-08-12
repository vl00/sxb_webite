using PMS.CommentsManage.Application.ModelDto.Reply;
using Sxb.UserCenter.Models.CommentViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    /// <summary>
    /// 点评回复 转 视图实体
    /// </summary>
    public static class ReplyDtoToViewModelHelper
    {
        public static List<CommentReplyViewModel> ReplyDtoToViewModel(List<ReplyDto> ReplyDtos,List<UserViewModel> UserInfoVos) 
        {
            List<CommentReplyViewModel> ReplyVo = new List<CommentReplyViewModel>();
            foreach (var item in ReplyDtos)
            {
                var u = UserInfoVos.FirstOrDefault(x => x.Id == item.ReplyUserId);

                UserViewModel ui = new UserViewModel
                {
                    Id = u.Id,
                    HeadImage= u.HeadImage,
                    UserName = u.UserName,
                    Role = u.Role
                };

                if (item.isAnonymou)
                {
                    ui.UserName = "匿名用户";
                    ui.HeadImage = "https://cos.sxkid.com/images/AppComment/defaultUserHeadImage.png";
                }

                CommentReplyViewModel model = new CommentReplyViewModel();
                model.CommentId = item.SchoolCommentId;
                model.Id = item.Id;
                model.IsLike = item.isLike;
                model.LikeTotal = item.LikeCount;
                model.ReplyTotal = item.ReplayCount;
                model.Content = item.Content;
                model.AddTime = item.AddTime;
                model.UserInfoVo = ui;
                model.IsAttend = item.isAttend;
                model.IsStudent = item.isStudent;
                model.IsAnonymou = item.isAnonymou;
                ReplyVo.Add(model);
            }
            return ReplyVo;
        }
    }

}
