using PMS.CommentsManage.Application.ModelDto.Reply;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    /// <summary>
    /// 点评回复 转 视图实体
    /// </summary>
    public static class ReplyDtoToViewModelHelper
    {
        public static List<CommentReplyViewModel> ReplyDtoToViewModel(List<ReplyDto> ReplyDtos, List<UserInfoVo> UserInfoVos)
        {
            List<CommentReplyViewModel> ReplyVo = new List<CommentReplyViewModel>();
            foreach (var item in ReplyDtos)
            {
                var u = UserInfoVos.FirstOrDefault(x => x.Id == item.ReplyUserId);

                UserInfoVo ui;
                if (u != null)
                {
                    ui = new UserInfoVo
                    {
                        Id = u.Id,
                        HeadImager = u.HeadImager,
                        NickName = u.NickName,
                        Role = u.Role
                    };
                }
                else
                {
                    ui = new UserInfoVo();
                }
                if (item.isAnonymou)
                {
                    ui.NickName = "匿名用户";
                    ui.HeadImager = "https://cos.sxkid.com/images/AppComment/defaultUserHeadImage.png";
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
